// ContextMenuWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace MyLLMIntegrationApp
{
    public partial class ContextMenuWindow : Window
    {
        public delegate void ActionSelectedHandler(ActionDefinition actionDef);
        public event ActionSelectedHandler? OnActionSelected;

        public ContextMenuWindow()
        {
            InitializeComponent();
        }

        public void BuildMenus(List<MenuDefinition> menus)
        {
            Console.WriteLine("Building menus");
            var stack = new StackPanel();
            foreach (var menu in menus)
            {
                // For top-level menus, we can use an Expander or a Label:
                var menuExpander = new Expander { Header = menu.Name, IsExpanded = true };
                menuExpander.Content = BuildSubmenuContent(menu.Submenus, menu.Actions);
                stack.Children.Add(menuExpander);
            }

            this.Content = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                CornerRadius = new System.Windows.CornerRadius(5),
                Padding = new Thickness(5),
                Child = stack
            };
        }

        private UIElement BuildSubmenuContent(List<SubmenuDefinition> submenus, List<ActionDefinition> actions)
        {
            var stack = new StackPanel();

            // Build submenus
            foreach (var submenu in submenus)
            {
                var subExpander = new Expander
                {
                    Header = submenu.Name,
                    IsExpanded = true
                };
                subExpander.Content = BuildSubmenuContent(submenu.Submenus, submenu.Actions);
                stack.Children.Add(subExpander);
            }

            // Build actions (buttons)
            foreach (var action in actions)
            {
                var button = new Button { Content = action.Name, Margin = new Thickness(5) };
                button.Click += (s, e) => {
                    this.Close(); 
                    OnActionSelected?.Invoke(action);
                };
                stack.Children.Add(button);
            }

            return stack;
        }

        public void ShowAtCursor()
        {
            var pt = System.Windows.Forms.Cursor.Position;
            this.Left = pt.X;
            this.Top = pt.Y;
            this.Show();
        }
    }
}
