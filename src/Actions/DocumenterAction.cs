using Logi.PluginCore.Attributes;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

/// <summary>
/// AI-powered code documentation generator
/// Generates inline comments and summary documentation for functions/classes
/// </summary>
[PluginAction]
public class DocumenterAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a documentation writer. Generate:

**Documented Code:** [original code with inline comments + docstrings]

Match the language style (JSDoc for JS/TS, XML for C#, docstrings for Python, etc.).
No essays—just add inline comments for complex logic and function/method docs.
<END>";

    public override string Name => "Documenter";
    public override string Description => "Generate inline comments and documentation";

    protected override void OnButtonPress()
    {
        _ = ExecuteDocumentAsync();
    }

    private async Task ExecuteDocumentAsync()
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
                UserPrompt = $"Add documentation to this code:\n\n{code}",
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
                ShowError($"Documentation failed: {response.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }
}
