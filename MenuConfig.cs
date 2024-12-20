// MenuConfig.cs
using System.Collections.Generic;

public class MenuDefinition
{
    public string Name { get; set; } = "";
    public List<SubmenuDefinition> Submenus { get; set; } = new();
    public List<ActionDefinition> Actions { get; set; } = new();
}

public class SubmenuDefinition
{
    public string Name { get; set; } = "";
    public List<SubmenuDefinition> Submenus { get; set; } = new();
    public List<ActionDefinition> Actions { get; set; } = new();
}

public class ActionDefinition
{
    public string Name { get; set; } = "";
    public Dictionary<string, string> Vars { get; set; } = new();
    public string Prompt { get; set; } = "";
    public bool RequiresUserInput { get; set; } = false;
    public string? UserInputPrompt { get; set; }
    public string? PromptIfNoSelectedText { get; set; }
    public string? FallbackVar { get; set; }
}
