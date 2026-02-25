using Loupedeck;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

/// <summary>
/// AI-powered code debugger — analyzes clipboard code and returns root cause + fix
/// </summary>
public class DebuggerAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a code debugger. Analyze the provided code/stack trace and respond with:

**Root Cause:** [1-2 sentence explanation]
**Minimal Fix:** [specific code change]
**Why:** [1-2 bullets]

If context is missing, state assumptions explicitly. Keep sections SHORT. Use code blocks only when needed.";

    public DebuggerAction()
        : base("AI Debugger", "Analyze and debug code from clipboard")
    { }

    protected override void RunCommand(string actionParameter)
    {
        _ = ExecuteAsync();
    }

    private async Task ExecuteAsync()
    {
        ShowLoading();
        try
        {
            var code = ClipboardService.GetText();
            if (string.IsNullOrWhiteSpace(code)) { ShowError("No code in clipboard"); return; }

            var settings = SettingsService.GetSettings();
            var response = await LLMService.SendRequestAsync(new LLMRequest
            {
                SystemPrompt = SystemPrompt,
                UserPrompt = $"Debug this code:\n\n{code}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 1200
            });

            if (response.Success) { ClipboardService.SetText(response.Content); ShowSuccess(); }
            else ShowError($"Debug failed: {response.Error}");
        }
        catch (Exception ex) { ShowError($"Error: {ex.Message}"); }
    }
}
