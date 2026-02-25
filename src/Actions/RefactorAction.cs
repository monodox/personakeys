using Loupedeck;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

public class RefactorAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a code refactorer. Provide:

**Refactored Code:** [improved version]
**Why It's Better:** [2-3 bullets: safer/cleaner/more idiomatic]

Return ONLY the refactored code and brief reasoning. No essays.";

    public RefactorAction()
        : base("AI Refactor", "Refactor and improve code from clipboard")
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
                UserPrompt = $"Refactor this code:\n\n{code}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 1200
            });

            if (response.Success) { ClipboardService.SetText(response.Content); ShowSuccess(); }
            else ShowError($"Refactor failed: {response.Error}");
        }
        catch (Exception ex) { ShowError($"Error: {ex.Message}"); }
    }
}
