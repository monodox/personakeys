using Loupedeck;

namespace PersonaKeys.Actions;

/// <summary>
/// Actions Ring adjustment — maps 0-100 dial position to LLM temperature/topP/repeatPenalty
/// </summary>
public class StrictnessAdjustmentAction : PluginDynamicAdjustment
{
    private int _strictness = 50;

    private PersonaKeysPlugin PluginInstance => (PersonaKeysPlugin)this.Plugin;

    public StrictnessAdjustmentAction()
        : base("Strictness", "Adjust AI strictness: 0=Deterministic, 100=Creative", "PersonaKeys", hasReset: true)
    { }

    protected override void ApplyAdjustment(string actionParameter, int diff)
    {
        _strictness = Math.Clamp(_strictness + diff, 0, 100);
        PersistSettings();
        this.AdjustmentValueChanged();
    }

    // Reset button sets to balanced (50)
    protected override void RunCommand(string actionParameter)
    {
        _strictness = 50;
        PersistSettings();
        this.AdjustmentValueChanged();
    }

    protected override string GetAdjustmentValue(string actionParameter)
    {
        string mode = _strictness switch
        {
            < 20 => "Strict",
            < 40 => "Conservative",
            < 60 => "Balanced",
            < 80 => "Creative",
            _ => "Experimental"
        };
        return $"{mode} ({_strictness})";
    }

    private void PersistSettings()
    {
        try
        {
            var (temp, topP, rp) = _strictness switch
            {
                < 20 => (0.1, 0.70, 1.05),
                < 40 => (0.3, 0.80, 1.08),
                < 60 => (0.5, 0.85, 1.10),
                < 80 => (0.7, 0.90, 1.15),
                _    => (0.9, 0.95, 1.20)
            };

            var svc = PluginInstance.GetSettingsService();
            var settings = svc.GetSettings();
            settings.Temperature = temp;
            settings.TopP = topP;
            settings.RepeatPenalty = rp;
            svc.SaveSettings(settings);
        }
        catch { /* plugin may not be fully loaded yet */ }
    }
}
