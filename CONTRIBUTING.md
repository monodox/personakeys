# Contributing to PersonaKeys

Thank you for your interest in contributing to PersonaKeys! This document provides guidelines and instructions for contributing.

## Development Setup

### Prerequisites
- **Basic knowledge of .NET and C# development**
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **A .NET IDE**:
  - Visual Studio Code
  - Visual Studio 2022 Community Edition or higher
  - JetBrains Rider
- **Logitech Options+** or **Loupedeck software**:
  - Logitech Options+: https://www.logitech.com/software/logi-options-plus.html
  - Loupedeck Software: https://loupedeck.com/downloads/
- **LogiPluginTool** (Logitech Actions SDK CLI):
  ```bash
  dotnet tool install --global LogiPluginTool
  ```
- **Compatible hardware device** for testing:
  - Logitech MX Creative Console
  - Loupedeck CT / Live / Live S
  - Razer Stream Controller
- **Optional**: Ollama for local AI testing

### Getting Started

1. **Fork and clone**:
   ```bash
   git clone https://github.com/yourusername/personakeys.git
   cd personakeys
   ```

2. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Build the plugin**:
   ```bash
   cd src
   dotnet build
   ```
   This creates a `.link` file in the Logi Plugin Service's Plugins directory.

4. **Verify plugin appears** in Logitech Options+ or Loupedeck software:
   - **Options+**: 'All Actions' → 'Installed Plugins' → 'PersonaKeys'
   - **Loupedeck**: Unhide in "Hide and show plugins" tab
   - If not visible: Restart Logi Plugin Service in settings

5. **Use Hot Reload for development**:
   ```bash
   cd src
   dotnet watch build
   ```
   Changes to source files automatically rebuild and reload the plugin.
   More info: https://devblogs.microsoft.com/dotnet/introducing-net-hot-reload/

6. **Test thoroughly** on actual hardware

7. **Submit a pull request**

## Project Architecture

### Key Components

**Plugin.cs**
- Main entry point
- Service initialization
- Plugin lifecycle management

**Actions/**
- Individual AI command implementations
- Each action inherits from `BasePersonaAction`
- Follows SDK conventions for button press, adjustments, etc.

**Services/**
- `LLMService`: Handles all LLM provider communication
- `ClipboardService`: Windows clipboard integration
- `SettingsService`: Plugin settings management

**Models/**
- Data structures and DTOs
- Settings classes
- Request/Response models

## Coding Standards

### C# Style Guide
- Follow standard C# naming conventions
- Use `PascalCase` for classes, methods, properties
- Use `camelCase` for private fields (with `_` prefix)
- Use meaningful variable names
- Add XML documentation comments for public APIs

### Example
```csharp
/// <summary>
/// Sends a request to the LLM provider
/// </summary>
/// <param name="request">The LLM request to send</param>
/// <returns>The LLM response</returns>
public async Task<LLMResponse> SendRequestAsync(LLMRequest request)
{
    // Implementation
}
```

## Adding New Actions

### Step 1: Create Action Class
Create a new file in `src/Actions/`

```csharp
using Logi.PluginCore.Attributes;
using PersonaKeys.Models;

namespace PersonaKeys.Actions;

[PluginAction]
public class MyNewAction : BasePersonaAction
{
    public override string Name => "My Action";
    public override string Description => "What this action does";

    protected override void OnButtonPress()
    {
        _ = ExecuteAsync();
    }

    private async Task ExecuteAsync()
    {
        ShowLoading();
        
        try
        {
            var input = ClipboardService.GetText();
            
            // Your logic here
            
            ShowSuccess();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }
}
```

### Step 2: Add Icon
- Create an icon in `assets/` directory
- Reference it in your action

### Step 3: Test
- Build the project
- Test on actual hardware
- Verify haptic feedback
- Test with different LLM providers

## Adding New LLM Providers

To add support for a new LLM provider:

1. Add a new method in `LLMService.cs`
2. Follow the existing pattern (Ollama, OpenAI, etc.)
3. Update `PluginSettings` model if needed
4. Handle authentication appropriately
5. Test thoroughly

## LLM Integration Details (Ollama)

### Default Configuration

**Runtime**: Ollama (local inference)  
**Model**: `llama3.2:latest` (default)  
**Transport**: HTTP to `http://127.0.0.1:11434`  
**API**: `/api/chat` (supports structured system/user messages)

**Why /api/chat over /api/generate?**
- Structured message format (system/user roles)
- Better for persona-based prompting
- Consistent with OpenAI/Anthropic APIs

### Ring-Driven Parameterization

Ring position (0-100) maps to multiple generation parameters:

| Parameter | Range | Purpose |
|-----------|-------|---------|
| `temperature` | 0.1 → 0.9 | Creativity/randomness |
| `top_p` | 0.7 → 0.95 | Sampling diversity |
| `repeat_penalty` | 1.05 → 1.2 | Reduces verbose repetition |

**Implementation**: See `StrictnessAdjustmentAction.MapStrictnessToParameters()`

### Persona Prompt Best Practices

To prevent llama3.2 verbosity:

1. **Use tight system prompts** with explicit constraints
2. **Specify output format** (e.g., "Root Cause: [explanation]")
3. **Add stop tokens**: `["\n\n---\n\n", "<END>"]` in system prompt
4. **Limit max tokens**: 800-1200 per persona (not 2048+)
5. **State assumptions explicitly** when context is missing

**Example** (Debugger persona):
```
Root Cause: [1-2 sentences]
Minimal Fix: [code change]
Why: [2 bullets]
<END>
```

### Error Handling

1. **Check model availability** on plugin load (`CheckOllamaModelAsync()`)
2. **Handle timeouts gracefully** (default 60s)
3. **Provide actionable error messages**:
   - Missing model: `"Run: ollama pull llama3.2:latest"`
   - Connection fail: Check endpoint reachability
4. **Never log clipboard contents** (privacy consideration)

## Testing Guidelines

### Manual Testing Checklist
- [ ] Action appears in device UI
- [ ] Button press triggers action correctly
- [ ] Loading state displays properly
- [ ] Success feedback works (visual + haptic)
- [ ] Error handling works correctly
- [ ] Clipboard integration functions
- [ ] Settings persist correctly
- [ ] Works with multiple LLM providers

### Hardware Testing
Always test on actual hardware before submitting:
- Logitech MX Creative Console
- Loupedeck devices
- Razer Stream Controller (if available)

## Packaging and Distribution

### Creating a Plugin Package

Before distributing PersonaKeys, package it as a `.lplug4` file:

```bash
# Build release version
cd src
dotnet build -c Release

# Package the plugin
logiplugintool pack ./bin/Release/ ./PersonaKeys.lplug4

# Verify package integrity
logiplugintool verify ./PersonaKeys.lplug4
```

**Package Requirements**:
- Plugin icon in `metadata/` subfolder
- Plugin configuration file: `metadata/LoupedeckPackage.yaml`
- Naming convention: `PersonaKeys_1.0.0.lplug4`
- `.lplug4` is a zip file with specific format

### Marketplace Submission

To submit to **Logitech Marketplace** and **Loupedeck Marketplace**:

1. **Pre-submission checklist**:
   - [ ] Tested with all supported hardware
   - [ ] Complies with Marketplace Approval Guidelines
   - [ ] Plugin icon included in metadata folder
   - [ ] LoupedeckPackage.yaml configured correctly
   - [ ] .lplug4 package passes verification
   - [ ] All features documented
   - [ ] Security best practices followed

2. **Submit plugin**:
   - Go to https://marketplace.logitech.com/contribute
   - Fill out submission form
   - Upload `.lplug4` package
   - Provide plugin description, screenshots, documentation links

3. **Installation**: Users can install by double-clicking the `.lplug4` file

### Testing Guidelines
- [ ] Action appears in device UI
- [ ] Button press triggers action correctly
- [ ] Loading state displays properly
- [ ] Success feedback works (visual + haptic)
- [ ] Error handling works correctly
- [ ] Clipboard integration functions
- [ ] Settings persist correctly
- [ ] Works with multiple LLM providers

### Hardware Testing
Always test on actual hardware before submitting:
- Logitech MX Creative Console
- Loupedeck devices
- Razer Stream Controller (if available)

## Submitting Changes

### Pull Request Process
1. Update README.md if you add features
2. Update CHANGELOG.md (if exists)
3. Test on actual hardware
4. Ensure code builds without errors
5. Submit PR with clear description

### PR Description Template
```markdown
## Description
Brief description of what this PR does

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Tested on hardware (specify device)
- [ ] Tested with Ollama
- [ ] Tested with OpenAI/Azure/Anthropic
- [ ] All actions still work

## Screenshots
If applicable, add screenshots or video
```

## Code Review

All submissions require review. We'll look for:
- Code quality and style
- Proper error handling
- Documentation
- Testing on hardware
- Performance considerations

## Feature Requests

Have an idea? Open an issue with:
- Clear description of the feature
- Use case / problem it solves
- Any implementation ideas
- Examples from other tools (if applicable)

## Bug Reports

Found a bug? Open an issue with:
- Steps to reproduce
- Expected behavior
- Actual behavior
- Device model
- Software versions
- Logs (if available)

## Community

- Be respectful and constructive
- Help others when you can
- Share your use cases and workflows
- Report bugs and suggest improvements

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing to PersonaKeys! 🚀
