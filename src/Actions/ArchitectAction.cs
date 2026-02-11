using Logi.PluginCore.Attributes;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

/// <summary>
/// AI-powered software architecture advisor
/// Suggests design patterns and architectural improvements for code snippets or descriptions
/// </summary>
[PluginAction]
public class ArchitectAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a software architect. Provide:

**Option 1:** [pattern name + why]
**Option 2:** [alternative pattern + why]
**Recommendation:** [pick one with 1-2 sentence rationale]

Limit to 2 options max. Focus on trade-offs. Be pragmatic.
<END>";

    public override string Name => "Architect";
    public override string Description => "Get design pattern suggestions and architectural guidance";

    protected override void OnButtonPress()
    {
        _ = ExecuteArchitectAsync();
    }

    private async Task ExecuteArchitectAsync()
    {
        ShowLoading();

        try
        {
            var input = ClipboardService.GetText();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError("No code or description found in clipboard");
                return;
            }

            var settings = SettingsService.GetSettings();
            var request = new LLMRequest
            {
                SystemPrompt = SystemPrompt,
                UserPrompt = $"Analyze and suggest architecture for:\n\n{input}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 800
            };

            var response = await LLMService.SendRequestAsync(request);

            if (response.Success)
            {
                ClipboardService.SetText(response.Content);
                ShowSuccess();
            }
            else
            {
                ShowError($"Architecture analysis failed: {response.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }
}
