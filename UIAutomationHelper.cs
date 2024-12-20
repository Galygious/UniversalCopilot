// UIAutomationHelper.cs
using System.Windows.Automation;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MyLLMIntegrationApp
{
    public static class UIAutomationHelper
    {
        public static string GetSelectedText()
		{
			var element = AutomationElement.FocusedElement;
			if (element == null) return "";

			// Check if the element supports TextPattern
			object patternObj;
			if (element.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
			{
				var textPattern = patternObj as TextPattern;
				if (textPattern != null)
				{
					var ranges = textPattern.GetSelection();
					if (ranges.Length > 0)
					{
						return ranges[0].GetText(-1);
					}
				}
			}

			// If TextPattern is not supported or no selection found, try ValuePattern
			object valuePatternObj;
			if (element.TryGetCurrentPattern(ValuePattern.Pattern, out valuePatternObj))
			{
				var valuePattern = valuePatternObj as ValuePattern;
				if (valuePattern != null)
				{
					// ValuePattern doesn't give a selection, just the full text
					// If user wants to rely on selection, fallback is needed.
					// For now, return entire text content:
					return valuePattern.Current.Value;
				}
			}

			// If no pattern supported, fallback to clipboard method:
			return GetSelectedTextViaClipboardFallback();
		}

        private static string GetSelectedTextViaClipboardFallback()
		{
			var originalClip = System.Windows.Forms.Clipboard.GetText();
			SendCtrlC();
			Thread.Sleep(100);

			string? result = null;
			const int maxRetries = 5;
			for (int i = 0; i < maxRetries; i++)
			{
				try
				{
					var temp = System.Windows.Forms.Clipboard.GetText();
					if (temp != originalClip) 
					{
						// We got something different from what was originally in the clipboard
						result = temp;
						break;
					}
				}
				catch (System.Runtime.InteropServices.ExternalException)
				{
					// Clipboard busy, wait and retry
				}
				Thread.Sleep(100);
			}

			// Restore the original clipboard content
			// (We do this after we've tried to get new text)
			System.Windows.Forms.Clipboard.SetText(originalClip);

			// If result is still null or still equals originalClip, no new text was obtained
			if (result == null || result == originalClip)
			{
				return ""; // No new text found, return empty string
			}

			return result;
		}


        private static void SendCtrlC()
        {
            SendKeyDown(Keys.ControlKey);
            SendKeyDown(Keys.C);
            SendKeyUp(Keys.C);
            SendKeyUp(Keys.ControlKey);
        }

		public static void InsertText(string text)
		{
			var element = AutomationElement.FocusedElement;
			if (element == null) return;

			// Check if the element supports ValuePattern
			object valuePatternObj;
			if (element.TryGetCurrentPattern(ValuePattern.Pattern, out valuePatternObj))
			{
				var valuePattern = valuePatternObj as ValuePattern;
				if (valuePattern != null && valuePattern.Current.IsReadOnly == false)
				{
					// Set the value if supported
					valuePattern.SetValue(text);
					return;
				} else if (valuePattern?.Current.IsReadOnly == true)
				{
					// If the element is read-only, we can't insert text into it.
					// For now, we'll just fall back to paste method.
					InsertTextViaPaste(text);
					return;
				}
			}

			// If we can’t insert via ValuePattern, fallback to paste method
			InsertTextViaPaste(text);
		}

		private static void InsertTextViaPaste(string text)
		{
			var originalClip = System.Windows.Forms.Clipboard.GetText();
			System.Windows.Forms.Clipboard.SetText(text);
			SendCtrlV();
			System.Threading.Thread.Sleep(100);
			System.Windows.Forms.Clipboard.SetText(originalClip);
		}

		private static void SendCtrlV()
		{
			SendKeyDown(Keys.ControlKey);
			SendKeyDown(Keys.V);
			SendKeyUp(Keys.V);
			SendKeyUp(Keys.ControlKey);
		}

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEYEVENTF_KEYUP = 0x0002;

        private static void SendKeyDown(Keys key)
        {
            keybd_event((byte)key, 0, 0, 0);
        }

        private static void SendKeyUp(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
