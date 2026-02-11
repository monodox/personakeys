using Logi.PluginCore.Attributes;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

/// <summary>
/// AI-powered code debugger action
/// Analyzes code from clipboard and provides debugging insights
/// </summary>
[PluginAction]
public class DebuggerAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a code debugger. Analyze the provided code/stack trace and respond with:

**Root Cause:** [1-2 sentence explanation]
**Minimal Fix:** [specific code change]
**Why:** [1-2 bullets]

If context is missing, state assumptions explicitly. Keep sections SHORT. Use code blocks only when needed.
<END>";

    public override string Name => "AI Debugger";
    public override string Description => "Analyze and debug code from clipboard";

    protected override void OnButtonPress()
    {
        _ = ExecuteDebugAsync();
    }

    private async Task ExecuteDebugAsync()
    {
        ShowLoading();

        try
        {
            // Get code from clipboard
            var code = ClipboardService.GetText();
            
            if (string.IsNullOrWhiteSpace(code))
            {
                ShowError("No code found in clipboard");
                return;
            }

            // Send to LLM with current strictness setting
            var settings = SettingsService.GetSettings();
            var request = new LLMRequest
            {
                SystemPrompt = SystemPrompt,
                UserPrompt = $"Debug this code:\n\n{code}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 1200
            };

            var response = await LLMService.SendRequestAsync(request);

            if (response.Success)
            {
                // Copy result back to clipboard
                ClipboardService.SetText(response.Content);
                ShowSuccess();
            }
            else
            {
                ShowError($"Debug failed: {response.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }
}
