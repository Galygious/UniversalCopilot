// NativeMenuHelper.cs
using System;
using System.Runtime.InteropServices;

public static class NativeMenuHelper
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool AppendMenuW(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int TrackPopupMenuEx(IntPtr hMenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr lpTPMParams);

    private const uint MF_STRING = 0x00000000;
    private const uint MF_POPUP = 0x00000010;
    private const uint TPM_RETURNCMD = 0x0100; 
    private const uint MF_SEPARATOR = 0x00000800;

    public static int ShowNativeContextMenu(IntPtr hWnd, int x, int y, IntPtr hMenu)
    {
        // TPM_RETURNCMD makes TrackPopupMenuEx return the command ID of the chosen item
        return TrackPopupMenuEx(hMenu, TPM_RETURNCMD, x, y, hWnd, IntPtr.Zero);
    }

    public static IntPtr CreateMenuFromDefinition(MenuDefinition menu, Dictionary<int, ActionDefinition> actionsMap, ref int idCounter)
    {
        IntPtr hMenu = CreatePopupMenu();

        // Add submenus
        foreach (var sub in menu.Submenus)
        {
            IntPtr hSubMenu = CreateMenuFromSubmenu(sub, actionsMap, ref idCounter);
            AppendMenuW(hMenu, MF_POPUP, (uint)hSubMenu, sub.Name);
        }

        // Add actions
        foreach (var action in menu.Actions)
        {
            // Assign an ID to this action
            int actionId = idCounter++;
            actionsMap[actionId] = action;
            AppendMenuW(hMenu, MF_STRING, (uint)actionId, action.Name);
        }

        return hMenu;
    }

    private static IntPtr CreateMenuFromSubmenu(SubmenuDefinition submenu, Dictionary<int, ActionDefinition> actionsMap, ref int idCounter)
    {
        IntPtr hMenu = CreatePopupMenu();

        // Add nested submenus
        foreach (var sub in submenu.Submenus)
        {
            IntPtr hSubMenu = CreateMenuFromSubmenu(sub, actionsMap, ref idCounter);
            AppendMenuW(hMenu, MF_POPUP, (uint)hSubMenu, sub.Name);
        }

        // Add actions
        foreach (var action in submenu.Actions)
        {
            int actionId = idCounter++;
            actionsMap[actionId] = action;
            AppendMenuW(hMenu, MF_STRING, (uint)actionId, action.Name);
        }

        return hMenu;
    }
}
