using System.Text.Json;
using PersonaKeys.Models;

namespace PersonaKeys.Services;

/// <summary>
/// Manages plugin settings via a local JSON file in AppData
/// </summary>
public class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Logi", "LogiPluginService", "PluginData", "PersonaKeys", "personakeys-settings.json");

    private PluginSettings? _cached;

    public PluginSettings GetSettings()
    {
        if (_cached != null) return _cached;

        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                _cached = JsonSerializer.Deserialize<PluginSettings>(json);
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Failed to load settings: {ex.Message}");
        }

        _cached ??= new PluginSettings();
        return _cached;
    }

    public void SaveSettings(PluginSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
            _cached = settings;
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Failed to save settings: {ex.Message}");
        }
    }
}
