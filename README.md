# PersonaKeys

**Transform your Logitech hardware into a physical AI command surface with specialized coding assistants.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Logitech Actions SDK](https://img.shields.io/badge/Logitech-Actions%20SDK-00B8FC)](https://developer.logi.com/actions-sdk)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS-lightgrey)](https://github.com/monodox/personakeys)

---

## About the project

### Inspiration

Developers constantly context-switch between their editor, terminal, and AI chat interfaces. We wanted to eliminate that friction entirely. The Logitech MX Creative Console and Loupedeck devices sit right on your desk with physical dials and buttons — why not make them your AI command surface? The idea was simple: press a button, get AI-enhanced code back in your clipboard. No windows, no tabs, no interruption to your flow state.

We were also inspired by the way hardware controls feel different from software sliders. Twisting a physical dial to shift an AI from "deterministic" to "experimental" mode is a fundamentally different experience than typing a parameter — it's tactile, immediate, and intuitive.

### What it does

PersonaKeys is a Logitech/Loupedeck plugin that turns your hardware device into a physical AI coding assistant. Four specialized developer personas are mapped to buttons:

- Debugger — analyzes code and stack traces, suggests root cause fixes
- Refactorer — improves code quality and readability
- Documenter — generates inline comments, XML docs, and JSDoc
- Architect — provides design pattern and structural guidance

The Actions Ring controls "strictness" in real time (0–100), which modulates temperature, top_p, and repeat_penalty simultaneously. Twist toward 0 for deterministic, proven solutions. Twist toward 100 for experimental rewrites. The workflow is: copy code → adjust ring → press persona button → paste result.

By default it runs fully local via Ollama — no API keys, no cloud, no cost per request.

### How we built it

Built in C# on .NET 8 using the official Logitech Actions SDK (Loupedeck SDK). Each persona is a `PluginDynamicCommand`, the strictness dial is a `PluginDynamicAdjustment`. Core services are cleanly separated — `LLMService` handles multi-provider inference, `ClipboardService` manages the copy/paste workflow, and `SettingsService` persists configuration to a local JSON file. The plugin packages as a `.lplug4` file for marketplace distribution.

### Challenges we ran into

The biggest challenge was the clipboard workflow on Windows — reliably reading and writing clipboard content from a background plugin process required careful thread marshaling and retry logic. Mapping a single dial value to multiple LLM parameters in a way that felt natural took several iterations. Haptic feedback timing was also tricky — the pulse on invocation needed to feel responsive without firing before the LLM call was dispatched.

### Accomplishments that we're proud of

The ring-driven multi-parameter modulation is the feature we're most proud of. Refactoring the same snippet at strictness 10 vs. 90 produces genuinely different results — from a minimal variable rename to a complete functional rewrite. That's a hardware-native behavior that doesn't exist anywhere else. We're also proud of the zero-friction workflow — from copy to paste, there's no UI to interact with.

### What we learned

Building for physical hardware forces you to think about feedback loops differently. In a GUI you can show a spinner; on a dial device you have haptics and that's it. We also learned a lot about LLM parameter sensitivity — small changes to repeat_penalty have outsized effects on output verbosity. The Loupedeck SDK is well-designed but sparsely documented, so we learned to read SDK source and the generated template project as our primary reference.

### What's next for PersonaKeys

Interactive settings UI, a persona SDK for community-defined personas in JSON/YAML, a persona marketplace, session memory, direct IDE integration, and persona packs beyond developers (Writer, Student, Marketer, Designer). Cross-platform support and hardware expansion beyond Logitech/Loupedeck are on the horizon for 2027.

---

## Key Features

- On-Device AI: Local inference via Ollama (private, no cloud required)
- 4 Specialized Developer Personas: Debugger, Refactorer, Documenter, Architect
- Physical Strictness Control: Actions Ring adjusts AI creativity (0–100 → multi-parameter mapping)
- Clipboard Workflow: Seamless copy-process-paste with no UI interruption
- Multi-Provider Support: Ollama (local), OpenAI, Azure OpenAI, Anthropic Claude
- Developer-First Design: Extensible architecture for future persona packs

---

## Quick Start

### Prerequisites

- .NET 8 SDK — [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- Logitech Options+ — [Download](https://www.logitech.com/software/logi-options-plus.html)
- Loupedeck Software — [Download](https://loupedeck.com/downloads/)
- A compatible device: Logitech MX Creative Console, Loupedeck CT / Live / Live S, or Razer Stream Controller
- [Ollama](https://ollama.ai) for local AI (recommended)

### Installation

1. Install LogiPluginTool:
   ```powershell
   dotnet tool install --global LogiPluginTool
   ```

2. Clone and build:
   ```powershell
   git clone https://github.com/monodox/personakeys.git
   cd personakeys/src
   dotnet build
   ```

3. The build automatically creates a `.link` file:
   - Windows: `C:\Users\USERNAME\AppData\Local\Logi\LogiPluginService\Plugins\PersonaKeys.link`
   - macOS: `~/Library/Application Support/Logi/LogiPluginService/Plugins/PersonaKeys.link`

4. Launch Logitech Options+ or Loupedeck software:
   - Options+: navigate to All Actions → Installed Plugins → PersonaKeys
   - Loupedeck: unhide PersonaKeys in the "Hide and show plugins" tab
   - If not visible, restart Logi Plugin Service in software settings

5. Pull the default model (if using Ollama):
   ```powershell
   ollama pull llama3.2:latest
   ```

6. Hot reload during development:
   ```powershell
   cd personakeys/src
   dotnet watch build
   ```

### Configuration

Settings are stored at:
`%LOCALAPPDATA%\Logi\LogiPluginService\PluginData\PersonaKeys\personakeys-settings.json`

**Ollama (local, recommended):**
```json
{
  "apiProvider": "ollama",
  "apiEndpoint": "http://127.0.0.1:11434",
  "model": "llama3.2:latest",
  "temperature": 0.7,
  "topP": 0.9,
  "repeatPenalty": 1.1,
  "enableHaptics": true,
  "timeout": 60
}
```

**OpenAI:**
```json
{
  "apiProvider": "openai",
  "apiKey": "sk-...",
  "model": "gpt-4o",
  "temperature": 0.7
}
```

**Anthropic:**
```json
{
  "apiProvider": "anthropic",
  "apiKey": "sk-ant-...",
  "model": "claude-3-5-sonnet-20241022",
  "temperature": 0.7
}
```

See [SECURITY.md](SECURITY.md) for API key safety guidelines.

---

## How It Works

### Developer Personas

| Persona | Button | What it does |
|---------|--------|--------------|
| Debugger | 🔍 | Analyzes code/stack traces, returns root cause + minimal fix |
| Refactorer | � | Improves code quality, readability, and performance |
| Documenter | 📝 | Generates inline comments, XML docs, JSDoc, Python docstrings |
| Architect | 🏗️ | Suggests design patterns and architectural trade-offs |

### Strictness Control

The Actions Ring maps a 0–100 position to three LLM parameters simultaneously:

| Position | Mode | Temperature | TopP | RepeatPenalty | Behavior |
|----------|------|-------------|------|---------------|----------|
| 0–20 | Strict | 0.1 | 0.70 | 1.05 | Deterministic, proven solutions |
| 20–40 | Conservative | 0.3 | 0.80 | 1.08 | Safe, incremental changes |
| 40–60 | Balanced | 0.5 | 0.85 | 1.10 | Pragmatic mix |
| 60–80 | Creative | 0.7 | 0.90 | 1.15 | Modern patterns |
| 80–100 | Experimental | 0.9 | 0.95 | 1.20 | Novel solutions, structural rewrites |

Twist the ring, press a persona — the same code produces visibly different output at strictness 10 vs. 90.

### Workflow

1. Copy code/text to clipboard
2. Adjust strictness via Actions Ring (optional)
3. Press a persona button
4. Paste the AI-enhanced result

No UI windows, no context switching — pure flow state.

---

## Project Structure

```
personakeys/
├── src/
│   ├── Actions/
│   │   ├── BasePersonaAction.cs        # Base class (PluginDynamicCommand)
│   │   ├── DebuggerAction.cs
│   │   ├── RefactorAction.cs
│   │   ├── DocumenterAction.cs
│   │   ├── ArchitectAction.cs
│   │   ├── StrictnessAdjustmentAction.cs  # PluginDynamicAdjustment (dial)
│   │   └── SettingsAction.cs
│   ├── Helpers/
│   │   └── PluginLog.cs                # SDK logging wrapper
│   ├── Services/
│   │   ├── LLMService.cs               # Multi-provider inference
│   │   ├── ClipboardService.cs         # Windows clipboard P/Invoke
│   │   └── SettingsService.cs          # JSON settings persistence
│   ├── Models/
│   │   └── Models.cs                   # DTOs and settings models
│   ├── Plugin.cs                       # Plugin entry point
│   ├── manifest.json                   # Plugin metadata
│   └── PersonaKeys.csproj
├── test/
│   └── OllamaIntegrationTest.cs
├── CHANGELOG.md
├── CONTRIBUTING.md
├── ROADMAP.md
├── SECURITY.md
└── LICENSE
```

---

## Distribution

### Package the plugin

```powershell
cd src
dotnet build -c Release
logiplugintool pack ./bin/Release/ ./PersonaKeys.lplug4
logiplugintool verify ./PersonaKeys.lplug4
```

Naming convention: `PersonaKeys_1.0.0.lplug4`. A `.lplug4` is a zip with a specific structure — double-clicking installs it to Logi Plugin Service.

### Marketplace submission

Submit to [Logitech Marketplace](https://marketplace.logitech.com/contribute):
1. Test with all supported hardware
2. Include plugin icon in `metadata/`
3. Package to `.lplug4` and verify
4. Submit at https://marketplace.logitech.com/contribute

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for architecture overview, adding new personas, coding standards, and the PR process.

## Roadmap

See [ROADMAP.md](ROADMAP.md) for the full timeline. Summary:
- Phase 1 (Q2 2026): Settings UI, enhanced feedback, developer experience
- Phase 2 (Q3 2026): Persona SDK, community packs, marketplace
- Phase 3 (Q4 2026): Context intelligence, IDE integration
- Phase 4 (Q1 2027): Enterprise features
- Phase 5 (Q2 2027): Cross-platform, hardware expansion

## Security

See [SECURITY.md](SECURITY.md) for vulnerability reporting and best practices. Use local Ollama for sensitive code — nothing leaves your machine.

## License

MIT — see [LICENSE](LICENSE).

## Support

- Issues: [GitHub Issues](https://github.com/monodox/personakeys/issues)
- Discussions: [GitHub Discussions](https://github.com/monodox/personakeys/discussions)

---

Built with the [Logitech Actions SDK](https://developer.logi.com/actions-sdk)
