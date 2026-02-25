# Contributing to PersonaKeys

Thanks for your interest in contributing. This document covers setup, architecture, coding standards, and the PR process.

---

## Development Setup

### Prerequisites

- .NET 8 SDK — [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- Logitech Options+ — [Download](https://www.logitech.com/software/logi-options-plus.html) or Loupedeck Software — [Download](https://loupedeck.com/downloads/)
- LogiPluginTool:
  ```powershell
  dotnet tool install --global LogiPluginTool
  ```
- A compatible device: Logitech MX Creative Console, Loupedeck CT / Live / Live S, or Razer Stream Controller
- Ollama for local AI testing — [Download](https://ollama.ai)

### Getting started

```powershell
git clone https://github.com/monodox/personakeys.git
cd personakeys/src
dotnet build
```

The build writes a `.link` file to `%LOCALAPPDATA%\Logi\LogiPluginService\Plugins\PersonaKeys.link` and sends a reload signal. Open Logitech Options+ or Loupedeck software — PersonaKeys should appear under installed plugins. If not, restart Logi Plugin Service in the software settings.

For hot reload during development:
```powershell
cd personakeys/src
dotnet watch build
```

---

## Architecture

```
Plugin.cs                    # Entry point — initializes services, checks Ollama on load
Helpers/PluginLog.cs         # Thin wrapper around SDK PluginLogFile
Actions/
  BasePersonaAction.cs       # Extends PluginDynamicCommand, exposes services
  DebuggerAction.cs          # Root cause + fix
  RefactorAction.cs          # Code quality improvements
  DocumenterAction.cs        # Inline comments + docstrings
  ArchitectAction.cs         # Design pattern suggestions
  StrictnessAdjustmentAction.cs  # PluginDynamicAdjustment — the dial
  SettingsAction.cs          # Logs current config on press
Services/
  LLMService.cs              # HTTP client for Ollama, OpenAI, Azure, Anthropic
  ClipboardService.cs        # Windows clipboard via P/Invoke
  SettingsService.cs         # JSON file persistence
Models/
  Models.cs                  # PluginSettings, LLMRequest, LLMResponse, PersonaCommand
```

### SDK types used

| SDK type | Used for |
|----------|----------|
| `Loupedeck.Plugin` | Plugin entry point |
| `Loupedeck.PluginDynamicCommand` | Button actions (personas) |
| `Loupedeck.PluginDynamicAdjustment` | Dial action (strictness ring) |
| `Loupedeck.PluginLogFile` | Logging |

### Settings file location

`%LOCALAPPDATA%\Logi\LogiPluginService\PluginData\PersonaKeys\personakeys-settings.json`

---

## Adding a New Persona

1. Create `src/Actions/MyPersonaAction.cs`:

```csharp
using Loupedeck;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

public class MyPersonaAction : BasePersonaAction
{
    private const string SystemPrompt = @"You are a [role]. Provide:
**Output:** [format]
Keep it SHORT.";

    public MyPersonaAction()
        : base("My Persona", "What this persona does")
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
            if (string.IsNullOrWhiteSpace(input)) { ShowError("No content in clipboard"); return; }

            var settings = SettingsService.GetSettings();
            var response = await LLMService.SendRequestAsync(new LLMRequest
            {
                SystemPrompt = SystemPrompt,
                UserPrompt = $"Process this:\n\n{input}",
                Temperature = settings.Temperature,
                TopP = settings.TopP,
                RepeatPenalty = settings.RepeatPenalty,
                MaxTokens = 1000
            });

            if (response.Success) { ClipboardService.SetText(response.Content); ShowSuccess(); }
            else ShowError($"Failed: {response.Error}");
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }
}
```

2. Build and test on hardware.

### Persona prompt guidelines

- Use tight system prompts with explicit output format
- Specify section headers (e.g., `**Root Cause:**`)
- Keep `MaxTokens` between 800–1200
- State assumptions explicitly when context is missing
- Avoid open-ended instructions that produce essays

---

## Adding a New LLM Provider

1. Add a private method in `LLMService.cs` following the existing pattern
2. Add the provider name to the switch in `SendRequestAsync`
3. Update `PluginSettings.ApiProvider` docs in `Models.cs`
4. Test with real credentials

---

## Coding Standards

- Follow standard C# conventions: `PascalCase` for types/methods/properties, `_camelCase` for private fields
- Add XML doc comments on public APIs
- Keep methods short and focused
- Handle exceptions at the action level — services throw, actions catch
- Never log clipboard contents

---

## Pull Request Process

1. Fork and create a feature branch: `git checkout -b feature/your-feature`
2. Build without errors: `dotnet build`
3. Test on actual hardware
4. Update `CHANGELOG.md` under `[Unreleased]`
5. Update `README.md` if you add user-facing features
6. Submit PR with this template:

```markdown
## Description
What this PR does

## Type of change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation

## Testing
- [ ] Tested on hardware (device model: ___)
- [ ] Tested with Ollama
- [ ] Tested with cloud provider (optional)
- [ ] All existing actions still work
```

---

## Bug Reports

Open an issue with:
- Steps to reproduce
- Expected vs actual behavior
- Device model and software version
- Relevant log output

## Feature Requests

Open an issue with:
- Clear description and use case
- Any implementation ideas
- Examples from other tools if applicable

---

By contributing, you agree your contributions will be licensed under the MIT License.
