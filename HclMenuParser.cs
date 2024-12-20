// HclMenuParser.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class HclMenuParser
{
    public List<MenuDefinition> Parse(string filePath)
    {
        var lines = File.ReadAllLines(filePath).Select(l => l.Trim()).ToList();
        var index = 0;
        var menus = new List<MenuDefinition>();

        while (index < lines.Count)
        {
            var line = lines[index];

            // Detect menu
            if (line.StartsWith("menu "))
            {
                var menu = ParseMenuBlock(lines, ref index);
                if (menu != null)
                    menus.Add(menu);
            }
            else
            {
                index++;
            }
        }

        return menus;
    }

    private MenuDefinition? ParseMenuBlock(List<string> lines, ref int index)
    {
        // line like: menu "MainMenu" {
        // Extract name with regex
        var line = lines[index];
        var menuName = ExtractName(line, "menu");
        index++;

        var menu = new MenuDefinition { Name = menuName };

        // Expect an opening { line is already processed, next lines until matching '}'
        // We'll parse submenus and actions inside
        while (index < lines.Count)
        {
            line = lines[index];
            if (line == "}")
            {
                // End of this menu
                index++;
                break;
            }
            else if (line.StartsWith("submenu "))
            {
                var submenu = ParseSubmenuBlock(lines, ref index);
                if (submenu != null)
                    menu.Submenus.Add(submenu);
            }
            else if (line.StartsWith("action "))
            {
                var action = ParseActionBlock(lines, ref index);
                if (action != null)
                    menu.Actions.Add(action);
            }
            else
            {
                index++;
            }
        }

        return menu;
    }

    private SubmenuDefinition? ParseSubmenuBlock(List<string> lines, ref int index)
    {
        // line like: submenu "Summaries" {
        var line = lines[index];
        var submenuName = ExtractName(line, "submenu");
        index++;

        var submenu = new SubmenuDefinition { Name = submenuName };

        while (index < lines.Count)
        {
            line = lines[index];
            if (line == "}")
            {
                index++;
                break;
            }
            else if (line.StartsWith("submenu "))
            {
                var sub = ParseSubmenuBlock(lines, ref index);
                if (sub != null)
                    submenu.Submenus.Add(sub);
            }
            else if (line.StartsWith("action "))
            {
                var action = ParseActionBlock(lines, ref index);
                if (action != null)
                    submenu.Actions.Add(action);
            }
            else
            {
                index++;
            }
        }

        return submenu;
    }

    private ActionDefinition? ParseActionBlock(List<string> lines, ref int index)
    {
        // line like: action "Summarize" {
        var line = lines[index];
        var actionName = ExtractName(line, "action");
        index++;

        var action = new ActionDefinition { Name = actionName };

        while (index < lines.Count)
        {
            line = lines[index];
            if (line == "}")
            {
                index++;
                break;
            }
            else if (line.StartsWith("vars = {"))
            {
                // Parse vars
                index++;
                while (index < lines.Count && lines[index] != "}")
                {
                    var varLine = lines[index];
                    // varLine like: var1 = "selectedText"
                    var varMatch = Regex.Match(varLine, @"(\w+)\s*=\s*""([^""]+)""");
                    if (varMatch.Success)
                    {
                        var varName = varMatch.Groups[1].Value;
                        var varVal = varMatch.Groups[2].Value;
                        action.Vars[varName] = varVal;
                    }
                    index++;
                }
                // consume closing '}'
                index++;
            }
            else if (line.StartsWith("prompt = "))
            {
                // prompt = "Summarize this:\n{var1}"
                var promptMatch = Regex.Match(line, @"prompt\s*=\s*""([^""]*)""");
                if (promptMatch.Success)
                {
                    action.Prompt = promptMatch.Groups[1].Value;
                }
                index++;
            }
            else if (line.StartsWith("requires_user_input = "))
            {
                var reqMatch = Regex.Match(line, @"requires_user_input\s*=\s*(\w+)");
                if (reqMatch.Success)
                {
                    action.RequiresUserInput = reqMatch.Groups[1].Value == "true";
                }
                index++;
            }
            else if (line.StartsWith("user_input_prompt = "))
            {
                var uipMatch = Regex.Match(line, @"user_input_prompt\s*=\s*""([^""]*)""");
                if (uipMatch.Success)
                {
                    action.UserInputPrompt = uipMatch.Groups[1].Value;
                }
                index++;
            }
			else if (line.StartsWith("prompt_if_no_selected_text = "))
			{
				var match = Regex.Match(line, @"prompt_if_no_selected_text\s*=\s*""([^""]*)""");
				if (match.Success)
				{
					action.PromptIfNoSelectedText = match.Groups[1].Value;
				}
				index++;
			}
			else if (line.StartsWith("fallback_var = "))
			{
				var match = Regex.Match(line, @"fallback_var\s*=\s*""([^""]+)""");
				if (match.Success)
				{
					action.FallbackVar = match.Groups[1].Value;
				}
				index++;
			}
            else
            {
                index++;
            }
        }

        return action;
    }

    private string ExtractName(string line, string keyword)
    {
        // line like: menu "MainMenu" {
        // Extract the quoted name
        var pattern = keyword + @"\s*""([^""]+)""";
        var match = Regex.Match(line, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return "";
    }
}
