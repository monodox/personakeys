using Logi.PluginCore.Attributes;

namespace PersonaKeys.Actions;

/// <summary>
/// Settings configuration action
/// Provides access to API key configuration, model selection, and preferences
/// </summary>
[PluginAction]
public class SettingsAction : BasePersonaAction
{
    public override string Name => "Settings";
    public override string Description => "Configure API provider, model, and preferences";

    protected override void OnButtonPress()
    {
        // Open settings panel/UI
        // For MVP: Display current settings and provide feedback
        var settings = SettingsService.GetSettings();
        
        var settingsInfo = $@"PersonaKeys Configuration
━━━━━━━━━━━━━━━━━━━━━━━━
Provider: {settings.ApiProvider}
Model: {settings.Model}
Strictness: {GetStrictnessLevel(settings.Temperature)}
Max Tokens: {settings.MaxTokens}
Haptics: {(settings.EnableHaptics ? "✓ Enabled" : "✗ Disabled")}
━━━━━━━━━━━━━━━━━━━━━━━━
Configure via plugin settings panel";

        ShowAlert(settingsInfo);
        Log.Info($"Current settings: Provider={settings.ApiProvider}, Model={settings.Model}, Temp={settings.Temperature}");
        
        // TODO: Implement proper settings UI with:
        // - API key field (secure input)
        // - Model selection dropdown
        // - Default strictness slider
        // - Local vs cloud toggle
        // This meets marketplace viability requirements
    }

    private string GetStrictnessLevel(double temperature)
    {
        return temperature switch
        {
            <= 0.2 => "Strict (Deterministic)",
            <= 0.4 => "Conservative",
            <= 0.6 => "Balanced",
            <= 0.8 => "Creative",
            _ => "Experimental"
        };
    }
}
