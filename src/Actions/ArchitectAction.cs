using Loupedeck;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

public class ArchitectAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a software architect. Provide:

**Option 1:** [pattern name + why]
**Option 2:** [alternative pattern + why]
**Recommendation:** [pick one with 1-2 sentence rationale]

Limit to 2 options max. Focus on trade-offs. Be pragmatic.";

    public ArchitectAction()
        : base("Architect", "Get design pattern suggestions and architectural guidance")
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
            var input = ClipboardService.GetText();
            if (string.IsNullOrWhiteSpace(input)) { ShowError("No code in clipboard"); return; }

            var settings = SettingsService.GetSettings();
            var response = await LLMService.SendRequestAsync(new LLMRequest
            {
                SystemPrompt = SystemPrompt,
                UserPrompt = $"Analyze and suggest architecture for:\n\n{input}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 800
            });

            if (response.Success) { ClipboardService.SetText(response.Content); ShowSuccess(); }
            else ShowError($"Architecture analysis failed: {response.Error}");
        }
        catch (Exception ex) { ShowError($"Error: {ex.Message}"); }
    }
}
