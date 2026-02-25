using Loupedeck;
using PersonaKeys.Services;

namespace PersonaKeys.Actions;

/// <summary>
/// Base class for all PersonaKeys button actions
/// </summary>
public abstract class BasePersonaAction : PluginDynamicCommand
{
    protected BasePersonaAction(string name, string description)
        : base(name, description, "PersonaKeys")
    { }

    protected PersonaKeysPlugin PluginInstance => (PersonaKeysPlugin)this.Plugin;
    protected LLMService LLMService => PluginInstance.GetLLMService();
    protected ClipboardService ClipboardService => PluginInstance.GetClipboardService();
    protected SettingsService SettingsService => PluginInstance.GetSettingsService();

    protected void ShowLoading() => this.ActionImageChanged();
    protected void ShowSuccess() => Task.Delay(1500).ContinueWith(_ => this.ActionImageChanged());
    protected void ShowError(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
            PluginLog.Error(message);
        Task.Delay(2000).ContinueWith(_ => this.ActionImageChanged());
    }
}
