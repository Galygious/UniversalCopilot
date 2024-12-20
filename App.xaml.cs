// App.xaml.cs
using System.Windows;
using System.Windows.Forms;
using System.IO;
using MyLLMIntegrationApp;

namespace Universal_Copilot
{
    public partial class App : System.Windows.Application
    {
        private NotifyIcon? _trayIcon;
        public static Action<string>? OnMenuActionSelected;
        public List<MenuDefinition> Menus { get; private set; } = new();
        private string _capturedSelectedText = "";
        private IntPtr _hiddenWindowHandle;


        public App()
        {
            OnMenuActionSelected += MenuActionSelected;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menus.hcl");
            var parser = new HclMenuParser();
            Menus = parser.Parse(configPath);


            if (Menus.Count == 0)
            {
                System.Windows.MessageBox.Show("No menus found in HCL file!");
            }

            GlobalHotkeyHandler.InstallHooks();
            GlobalHotkeyHandler.OnHotkeyTriggered += OnGlobalHotkey;

            // Setup tray icon as before...
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = System.Drawing.SystemIcons.Application;
            _trayIcon.Visible = true;
            _trayIcon.Text = "LLM Integration Running";
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (sender, args) => {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                this.Shutdown();
            };
            contextMenu.Items.Add(exitItem);
            _trayIcon.ContextMenuStrip = contextMenu;

            // Create a hidden window to provide a handle for native menus
            var hiddenWindow = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Visibility = Visibility.Hidden
            };
            hiddenWindow.Show();
            hiddenWindow.Hide();

            // Get the HWND of this hidden window
            var helper = new System.Windows.Interop.WindowInteropHelper(hiddenWindow);
            _hiddenWindowHandle = helper.Handle;
        }

        private void OnGlobalHotkey()
        {
            Current.Dispatcher.Invoke(() =>
            {
                // Now we're on the main UI thread
                try
                {
                    _capturedSelectedText = MyLLMIntegrationApp.UIAutomationHelper.GetSelectedText() ?? "";
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    // Log or show message
                    _capturedSelectedText = "";
                    Console.WriteLine("Failed to get selected text: " + ex.Message);
                }

                var actionsMap = new Dictionary<int, ActionDefinition>();
                int idCounter = 1;

                if (Menus.Count == 0)
                {
                    return; // No menus to show
                }

                var mainMenu = Menus[0];
                IntPtr hMenu = NativeMenuHelper.CreateMenuFromDefinition(mainMenu, actionsMap, ref idCounter);

                // Get cursor position
                var pt = System.Windows.Forms.Cursor.Position;

                int chosenCmd = NativeMenuHelper.ShowNativeContextMenu(_hiddenWindowHandle, pt.X, pt.Y, hMenu);

                if (chosenCmd != 0 && actionsMap.TryGetValue(chosenCmd, out var selectedAction))
                {
                    HandleActionSelected(selectedAction);
                }
            });
        }


        private async void HandleActionSelected(ActionDefinition actionDef)
        {
            if (actionDef.Name == "Exit")
            {
                return;
            }

            string selectedText = _capturedSelectedText;

            // Check if no selected text and we have a prompt for user input defined
            if (string.IsNullOrWhiteSpace(selectedText) 
                && !string.IsNullOrEmpty(actionDef.PromptIfNoSelectedText) 
                && !string.IsNullOrEmpty(actionDef.FallbackVar))
            {
                // Prompt user for input
                string userEnteredText = Microsoft.VisualBasic.Interaction.InputBox(actionDef.PromptIfNoSelectedText, "Input Required", "");
                if (string.IsNullOrWhiteSpace(userEnteredText))
                    return; // user canceled

                // Override the selectedText variable used later
                selectedText = userEnteredText;
            }

            string userInput = "";
            if (actionDef.RequiresUserInput && !string.IsNullOrEmpty(actionDef.UserInputPrompt))
            {
                userInput = Microsoft.VisualBasic.Interaction.InputBox(actionDef.UserInputPrompt, "Input Required", "");
                if (string.IsNullOrWhiteSpace(userInput))
                    return; // user canceled
            }

            // Build the prompt
            string prompt = actionDef.Prompt;
            foreach (var kvp in actionDef.Vars)
            {
                string placeholder = "{" + kvp.Key + "}";
                string value = kvp.Value == "selectedText" ? selectedText :
                            kvp.Value == "userInput" ? userInput :
                            "";
                prompt = prompt.Replace(placeholder, value);
            }

            string response = await MyLLMIntegrationApp.LLMClient.GetCompletionAsync(prompt);
            if (!string.IsNullOrWhiteSpace(response))
            {
                MyLLMIntegrationApp.UIAutomationHelper.InsertText(response);
            }
        }

        private async void MenuActionSelected(string prompt)
        {
            // If still using this for some reason, 
            // or if you handle OnMenuActionSelected differently
            // Just leave it here if needed:
            string response = await LLMClient.GetCompletionAsync(prompt);
            if (!string.IsNullOrWhiteSpace(response))
            {
                UIAutomationHelper.InsertText(response);
            }
        }
    }
}
