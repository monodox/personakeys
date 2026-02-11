using System.Text.Json;
using PersonaKeys.Models;
using Logi.PluginCore;

namespace PersonaKeys.Services;

/// <summary>
/// Manages plugin settings storage and retrieval
/// </summary>
public class SettingsService
{
    private readonly BasePlugin _plugin;
    private PluginSettings? _cachedSettings;
    private const string SettingsKey = "PersonaKeysSettings";

    public SettingsService(BasePlugin plugin)
    {
        _plugin = plugin;
    }

    /// <summary>
    /// Gets the current plugin settings
    /// </summary>
    public PluginSettings GetSettings()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        try
        {
            var settingsJson = _plugin.GetPluginSetting(SettingsKey);
            
            if (!string.IsNullOrEmpty(settingsJson))
            {
                _cachedSettings = JsonSerializer.Deserialize<PluginSettings>(settingsJson);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load settings: {ex.Message}");
        }

        _cachedSettings ??= new PluginSettings();
        return _cachedSettings;
    }

    /// <summary>
    /// Saves plugin settings
    /// </summary>
    public void SaveSettings(PluginSettings settings)
    {
        try
        {
            var settingsJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            _plugin.SetPluginSetting(SettingsKey, settingsJson);
            _cachedSettings = settings;
            
            Log.Info("Settings saved successfully");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to save settings: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Resets settings to default
    /// </summary>
    public void ResetSettings()
    {
        _cachedSettings = new PluginSettings();
        SaveSettings(_cachedSettings);
    }
}
