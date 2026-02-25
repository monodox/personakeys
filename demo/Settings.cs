using System.Text.Json;
using System.Text.Json.Serialization;

namespace PersonaKeys.Demo;

public class Settings
{
    [JsonPropertyName("apiProvider")]  public string ApiProvider  { get; set; } = "ollama";
    [JsonPropertyName("apiKey")]       public string ApiKey       { get; set; } = "";
    [JsonPropertyName("apiEndpoint")]  public string ApiEndpoint  { get; set; } = "http://127.0.0.1:11434";
    [JsonPropertyName("model")]        public string Model        { get; set; } = "llama3.2:latest";
    [JsonPropertyName("temperature")]  public double Temperature  { get; set; } = 0.5;
    [JsonPropertyName("topP")]         public double TopP         { get; set; } = 0.85;
    [JsonPropertyName("repeatPenalty")]public double RepeatPenalty{ get; set; } = 1.1;
    [JsonPropertyName("maxTokens")]    public int    MaxTokens    { get; set; } = 2048;
    [JsonPropertyName("timeout")]      public int    Timeout      { get; set; } = 60;

    private static readonly string SettingsPath = Path.Combine(
        AppContext.BaseDirectory, "demo-settings.json");

    public static Settings Load()
    {
        if (File.Exists(SettingsPath))
        {
            try
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            catch { }
        }
        // Also check plugin settings path
        var pluginPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Logi", "LogiPluginService", "PluginData", "PersonaKeys", "personakeys-settings.json");
        if (File.Exists(pluginPath))
        {
            try
            {
                var json = File.ReadAllText(pluginPath);
                return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            catch { }
        }
        return new Settings();
    }

    public (double temp, double topP, double rp) MapStrictness(int strictness) => strictness switch
    {
        < 20 => (0.1, 0.70, 1.05),
        < 40 => (0.3, 0.80, 1.08),
        < 60 => (0.5, 0.85, 1.10),
        < 80 => (0.7, 0.90, 1.15),
        _    => (0.9, 0.95, 1.20)
    };

    public string StrictnessMode(int s) => s switch
    {
        < 20 => "Strict",
        < 40 => "Conservative",
        < 60 => "Balanced",
        < 80 => "Creative",
        _    => "Experimental"
    };
}
