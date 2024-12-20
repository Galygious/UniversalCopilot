using System;
using System.IO;
using Tomlyn;
using Tomlyn.Syntax;

public class AppConfig
{
    public string BaseAddress { get; set; } = "http://localhost:1234";
    public string ModelName { get; set; } = "qwen2.5-coder-3b-instruct@q4_0";
}
public static class ConfigLoader
{
    private const string ConfigFileName = "config.toml";

    public static AppConfig LoadConfig()
    {
        var config = new AppConfig();

        if (File.Exists(ConfigFileName))
        {
            try
            {
                var toml = File.ReadAllText(ConfigFileName);
                var parsed = Toml.ToModel<AppConfig>(toml);

                if (parsed != null)
                {
                    config = parsed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config.toml: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Config file '{ConfigFileName}' not found. Using defaults.");
        }

        return config;
    }
}
