using Loupedeck;

namespace PersonaKeys.Actions;

public class SettingsAction : BasePersonaAction
{
    public SettingsAction()
        : base("Settings", "Show current PersonaKeys configuration")
    { }

    protected override void RunCommand(string actionParameter)
    {
        var settings = SettingsService.GetSettings();
        PluginLog.Info($"PersonaKeys: Provider={settings.ApiProvider}, Model={settings.Model}, Temp={settings.Temperature}");
        ShowSuccess();
    }
}
