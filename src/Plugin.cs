using Loupedeck;
using PersonaKeys.Services;

namespace PersonaKeys;

/// <summary>
/// PersonaKeys - Hardware-native AI command surface for developers.
/// Personas: Debugger, Refactorer, Documenter, Architect
/// Actions Ring controls Strictness/Creativity (0-100)
/// </summary>
public class PersonaKeysPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    private LLMService? _llmService;
    private ClipboardService? _clipboardService;
    private SettingsService? _settingsService;

    public PersonaKeysPlugin()
    {
        PluginLog.Init(this.Log);
    }

    public override void Load()
    {
        _settingsService = new SettingsService();
        _llmService = new LLMService(_settingsService);
        _clipboardService = new ClipboardService();

        PluginLog.Info("PersonaKeys loaded. Personas: Debugger, Refactorer, Documenter, Architect");

        _ = CheckOllamaModelAsync();
    }

    private async Task CheckOllamaModelAsync()
    {
        try
        {
            var settings = _settingsService?.GetSettings();
            if (settings?.ApiProvider.ToLowerInvariant() == "ollama")
            {
                var available = await _llmService!.CheckOllamaModelAsync();
                if (!available)
                {
                    PluginLog.Warning($"Ollama model '{settings.Model}' not found. Run: ollama pull {settings.Model}");
                    _clipboardService?.SetText($"Run: ollama pull {settings.Model}");
                }
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Failed to check Ollama model: {ex.Message}");
        }
    }

    public override void Unload()
    {
        _llmService?.Dispose();
        _clipboardService?.Dispose();
        PluginLog.Info("PersonaKeys unloaded");
    }

    public LLMService GetLLMService() => _llmService ?? throw new InvalidOperationException("LLM Service not initialized");
    public ClipboardService GetClipboardService() => _clipboardService ?? throw new InvalidOperationException("Clipboard Service not initialized");
    public SettingsService GetSettingsService() => _settingsService ?? throw new InvalidOperationException("Settings Service not initialized");
}
