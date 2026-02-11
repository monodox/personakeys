using Logi.PluginCore.Attributes;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

/// <summary>
/// Strictness adjustment action using the Actions Ring
/// Controls AI behavior from Deterministic (0) to Creative (100)
/// Maps ring position to model temperature for real-time behavioral modulation
/// </summary>
[PluginAction]
public class StrictnessAdjustmentAction : BasePersonaAction
{
    private int _strictness = 50; // 0-100 ring value
    private double _temperature = 0.5;
    private double _topP = 0.85;
    private double _repeatPenalty = 1.1;

    [PluginActionParameter]
    public int Strictness
    {
        get => _strictness;
        set
        {
            _strictness = Math.Clamp(value, 0, 100);
            
            // Map ring value (0-100) to multiple parameters
            (_temperature, _topP, _repeatPenalty) = MapStrictnessToParameters(_strictness);
            
            // Update display
            string mode = _strictness switch
            {
                < 20 => "Strict",
                < 40 => "Conservative",
                < 60 => "Balanced",
                < 80 => "Creative",
                _ => "Experimental"
            };
            
            SetTitle($"{mode} ({_strictness})");
            
            // Send light tick haptic feedback on adjustment
            var settings = SettingsService.GetSettings();
            if (settings.EnableHaptics)
            {
                SendHapticFeedback(HapticPattern.Adjustment);
            }
        }
    }

    public override string Name => "Strictness";
    public override string Description => "Adjust AI strictness: 0=Deterministic, 100=Creative";

    /// <summary>
    /// Maps strictness ring value (0-100) to generation parameters
    /// Temperature: 0.1 → 0.9 (deterministic → creative)
    /// TopP: 0.7 → 0.95 (focused → diverse sampling)
    /// RepeatPenalty: 1.05 → 1.2 (minimal → aggressive penalty to reduce repetition)
    /// </summary>
    private (double temperature, double topP, double repeatPenalty) MapStrictnessToParameters(int strictness)
    {
        return strictness switch
        {
            < 20 => (0.1, 0.7, 1.05),    // Strict: deterministic, focused
            < 40 => (0.3, 0.8, 1.08),    // Conservative: careful, reliable
            < 60 => (0.5, 0.85, 1.1),    // Balanced: pragmatic mix
            < 80 => (0.7, 0.9, 1.15),    // Creative: modern approaches
            _ => (0.9, 0.95, 1.2)        // Experimental: novel solutions
        };
    }

    protected override void OnButtonPress()
    {
        // Save current strictness parameters to settings
        var settings = SettingsService.GetSettings();
        settings.Temperature = _temperature;
        settings.TopP = _topP;
        settings.RepeatPenalty = _repeatPenalty;
        SettingsService.SaveSettings(settings);
        
        ShowSuccess();
        ShowAlert($"Strictness: {_strictness} (T={_temperature:F1}, P={_topP:F2}, RP={_repeatPenalty:F2})");
        
        Log.Info($"Strictness set to {_strictness} (T={_temperature:F2}, TopP={_topP:F2}, RepeatPenalty={_repeatPenalty:F2})");
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        // Load saved temperature and reverse-map to strictness
        var settings = SettingsService.GetSettings();
        _temperature = settings.Temperature;
        _topP = settings.TopP;
        _repeatPenalty = settings.RepeatPenalty;
        
        // Reverse map temperature to strictness for display
        _strictness = _temperature switch
        {
            <= 0.1 => 10,
            <= 0.3 => 30,
            <= 0.5 => 50,
            <= 0.7 => 70,
            _ => 90
        };
        
        Strictness = _strictness; // Trigger display update
    }
}
