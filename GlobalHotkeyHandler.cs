// GlobalHotkeyHandler.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Universal_Copilot
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    public static class GlobalHotkeyHandler
    {
        private static IntPtr _mouseHookID = IntPtr.Zero;
        private static IntPtr _keyboardHookID = IntPtr.Zero;
        
        private static LowLevelMouseProc _mouseProc = MouseHookCallback;
        private static LowLevelKeyboardProc _kbProc = KbHookCallback;

        private static bool ctrlDown = false;
        private static bool shiftDown = false;

        const int WH_MOUSE_LL = 14;
        const int WH_KEYBOARD_LL = 13;
        const int WM_RBUTTONDOWN = 0x204;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        public static event Action? OnHotkeyTriggered; 

        public static void InstallHooks()
        {
            _keyboardHookID = SetHook(_kbProc, WH_KEYBOARD_LL);
            _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);
        }

        private static IntPtr SetHook(Delegate proc, int hookType)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(hookType, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr KbHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var keyAdjusted = vkCode - 44;
                if ((int)wParam == WM_KEYDOWN)
                {
                    if ((Key)keyAdjusted == Key.LeftCtrl || (Key)keyAdjusted == Key.RightCtrl)
                    {
                        ctrlDown = true;
                    }
                    if ((Key)keyAdjusted == Key.LeftShift || (Key)keyAdjusted == Key.RightShift)
                    {
                        shiftDown = true;
                    }
                }
                else if ((int)wParam == WM_KEYUP)
                {
                    if ((Key)keyAdjusted == Key.LeftCtrl || (Key)keyAdjusted == Key.RightCtrl)
                    {
                        ctrlDown = false;
                    }
                    if ((Key)keyAdjusted == Key.LeftShift || (Key)keyAdjusted == Key.RightShift)
                    {
                        shiftDown = false;
                    }
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = (int)wParam; 
                var mouseStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;

                if (msg == WM_RBUTTONDOWN)
                {
                    // If Ctrl+Shift are held and user right-clicks
                    if (ctrlDown && shiftDown)
                    {
                        // Trigger event instead of creating the menu here
                        OnHotkeyTriggered?.Invoke();

                        // Consume the click
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
