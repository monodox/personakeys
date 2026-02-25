using Loupedeck;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

public class DocumenterAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a documentation writer. Generate:

**Documented Code:** [original code with inline comments + docstrings]

Match the language style (JSDoc for JS/TS, XML for C#, docstrings for Python, etc.).
No essays—just add inline comments for complex logic and function/method docs.";

    public DocumenterAction()
        : base("Documenter", "Generate inline comments and documentation")
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
                UserPrompt = $"Add documentation to this code:\n\n{code}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 1200
            });

            if (response.Success) { ClipboardService.SetText(response.Content); ShowSuccess(); }
            else ShowError($"Documentation failed: {response.Error}");
        }
        catch (Exception ex) { ShowError($"Error: {ex.Message}"); }
    }
}
