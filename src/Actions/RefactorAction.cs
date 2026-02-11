using Logi.PluginCore.Attributes;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

/// <summary>
/// AI-powered code refactoring action
/// Improves code quality, readability, and performance
/// </summary>
[PluginAction]
public class RefactorAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a code refactorer. Provide:

**Refactored Code:** [improved version]
**Why It's Better:** [2-3 bullets: safer/cleaner/more idiomatic]

Return ONLY the refactored code and brief reasoning. No essays.
<END>";

    public override string Name => "AI Refactor";
    public override string Description => "Refactor and improve code from clipboard";

    protected override void OnButtonPress()
    {
        _ = ExecuteRefactorAsync();
    }

    private async Task ExecuteRefactorAsync()
    {
        ShowLoading();

        try
        {
            var code = ClipboardService.GetText();
            
            if (string.IsNullOrWhiteSpace(code))
            {
                ShowError("No code found in clipboard");
                return;
            }

            var settings = SettingsService.GetSettings();
            var request = new LLMRequest
            {
                SystemPrompt = SystemPrompt,
                UserPrompt = $"Refactor this code:\n\n{code}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 1200
            };

            var response = await LLMService.SendRequestAsync(request);

            if (response.Success)
            {
                ClipboardService.SetText(response.Content);
                ShowSuccess();
            }
            else
            {
                ShowError($"Refactor failed: {response.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }
}
