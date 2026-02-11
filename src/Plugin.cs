using Logi.PluginCore;
using Logi.PluginCore.Attributes;
using PersonaKeys.Services;

namespace PersonaKeys;

/// <summary>
/// PersonaKeys - Hardware-native AI command surface for developers.
/// 
/// ARCHITECTURE:
/// PersonaKeys provides a Developer Persona Pack with 4 specialized coding assistants,
/// designed to expand via downloadable persona packs for other use cases.
/// 
/// DEVELOPER PERSONAS:
/// - Debugger: Root cause analysis + fix suggestions
/// - Refactorer: Code optimization (strictness-aware)
/// - Documenter: Inline comments + comprehensive docs
/// - Architect: Design pattern suggestions
/// 
/// KEY INNOVATION:
/// Actions Ring controls Strictness/Creativity (0-100), producing visibly different
/// AI outputs from conservative to aggressive.
/// 
/// HAPTIC FEEDBACK:
/// - Ring turn → tactile tick
/// - AI invoked → pulse
/// - Success → confirmation vibration
/// - Error → distinct error vibration
/// 
/// EXTENSIBILITY:
/// Future persona packs can target writers, students, marketers, designers, etc.
/// </summary>
[Plugin]
public class Plugin : BasePlugin
{
    private LLMService? _llmService;
    private ClipboardService? _clipboardService;
    private SettingsService? _settingsService;

    public override void Load()
    {
        // Initialize services
        _settingsService = new SettingsService(this);
        _llmService = new LLMService(_settingsService);
        _clipboardService = new ClipboardService();

        Log.Info("PersonaKeys loaded successfully");
        Log.Info("Available personas: Debugger, Refactorer, Documenter, Architect");
        
        // Check Ollama model availability on startup (non-blocking)
        _ = CheckOllamaModelAsync();
    }
    
    private async Task CheckOllamaModelAsync()
    {
        try
        {
            var settings = _settingsService?.GetSettings();
            if (settings?.ApiProvider.ToLowerInvariant() == "ollama")
            {
                var modelAvailable = await _llmService!.CheckOllamaModelAsync();
                if (!modelAvailable)
                {
                    Log.Warning($"Ollama model '{settings.Model}' not found. Run: ollama pull {settings.Model}");
                    _clipboardService?.SetText($"Run: ollama pull {settings.Model}");
                }
                else
                {
                    Log.Info($"Ollama model '{settings.Model}' is available");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to check Ollama model: {ex.Message}");
        }
    }

    public override void Unload()
    {
        _llmService?.Dispose();
        _clipboardService?.Dispose();
        
        Log.Info("PersonaKeys plugin unloaded");
    }

    public LLMService GetLLMService() => _llmService ?? throw new InvalidOperationException("LLM Service not initialized");
    public ClipboardService GetClipboardService() => _clipboardService ?? throw new InvalidOperationException("Clipboard Service not initialized");
    public SettingsService GetSettingsService() => _settingsService ?? throw new InvalidOperationException("Settings Service not initialized");
}
