# PersonaKeys

**Transform your Logitech hardware into a physical AI command surface with specialized coding assistants.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Logitech Actions SDK](https://img.shields.io/badge/Logitech-Actions%20SDK-00B8FC)](https://developer.logi.com/actions-sdk)

PersonaKeys is a Logitech Actions SDK plugin that provides hardware-native AI coding assistants with physical strictness control. Map specialized AI personas to hardware buttons and use the Actions Ring to modulate AI behavior in real-time.

**PersonaKeys supports on-device inference via Ollama (llama3.2:latest), enabling private, low-latency AI actions without cloud dependencies.**

## ✨ Key Features

- **On-Device AI**: Local inference via Ollama (private, no cloud required)
- **4 Specialized Developer Personas**: Debugger, Refactorer, Documenter, Architect
- **Physical Strictness Control**: Actions Ring adjusts AI creativity (0-100 → multi-parameter mapping)
- **Haptic Feedback**: Tactile response for every interaction (tick, pulse, confirmation, error)
- **Clipboard Workflow**: Seamless copy-process-paste with no UI interruption
- **Multi-Provider Support**: Ollama (local), OpenAI, Azure OpenAI, Anthropic Claude
- **Developer-First Design**: Extensible architecture for future persona packs

## 🚀 Quick Start

### Prerequisites

- **Basic knowledge of .NET and C# development**
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Logitech Options+** (for MX Creative Console) - [Download](https://www.logitech.com/software/logi-options-plus.html)
- **Loupedeck Software** (for Loupedeck devices) - [Download](https://loupedeck.com/downloads/)
- **A compatible device** for testing:
  - Logitech MX Creative Console
  - Loupedeck CT / Live / Live S
  - Razer Stream Controller
- **Optional**: [Ollama](https://ollama.ai) for local AI (recommended)

### Installation

1. **Install LogiPluginTool** (Logitech Actions SDK command-line tool):
   ```powershell
   dotnet tool install --global LogiPluginTool
   ```

2. **Clone and build PersonaKeys**:
   ```powershell
   git clone https://github.com/monodox/personakeys.git
   cd personakeys/src
   dotnet build
   ```

3. **Verify plugin installation**:
   - The build creates a `.link` file in:
     - Windows: `C:\Users\USERNAME\AppData\Local\Logi\LogiPluginService\Plugins\PersonaKeys.link`
     - macOS: `~/Library/Application Support/Logi/LogiPluginService/Plugins/PersonaKeys.link`

4. **Launch Logitech Options+ or Loupedeck software**:
   - **Options+**: Navigate to 'All Actions' → 'Installed Plugins' → 'PersonaKeys'
   - **Loupedeck**: Unhide PersonaKeys in "Hide and show plugins" tab
   - If not visible, restart Logi Plugin Service in software settings

5. **Hot reload during development**:
   ```powershell
   cd personakeys/src
   dotnet watch build
   ```
   Changes to source files automatically rebuild and reload the plugin.

### Configuration

Settings are stored in `personakeys-settings.json` in your plugin data directory.

**Ollama (Local, Recommended)**:
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

**OpenAI**:
```json
{
  "apiProvider": "openai",
  "apiKey": "sk-...",
  "model": "gpt-4",
  "temperature": 0.7
}
```

See [SECURITY.md](SECURITY.md) for API key safety guidelines.

## 🎯 How It Works

### Developer Personas

Each persona is optimized for specific coding tasks:

#### 1. **Debugger** 🔍
Analyzes code and stack traces to provide root cause analysis and fix suggestions.

#### 2. **Refactorer** 🔧
Improves code quality, readability, and performance with strictness-aware refactoring.

#### 3. **Documenter** 📝
Generates inline comments, JSDoc, XML comments, and comprehensive documentation.

#### 4. **Architect** 🏗️
Provides design pattern suggestions and architectural guidance.

### Strictness Control (Signature Feature)

The Actions Ring provides real-time control over AI behavior through multi-parameter modulation:

| Ring Position | Mode | Temperature | TopP | RepeatPenalty | Behavior |
|---------------|------|-------------|------|---------------|----------|
| 0-20 | Strict | 0.1 | 0.7 | 1.05 | Deterministic, proven solutions |
| 20-40 | Conservative | 0.3 | 0.8 | 1.08 | Safe, incremental changes |
| 40-60 | Balanced | 0.5 | 0.85 | 1.1 | Pragmatic mix of approaches |
| 60-80 | Creative | 0.7 | 0.9 | 1.15 | Modern patterns, refactoring |
| 80-100 | Experimental | 0.9 | 0.95 | 1.2 | Novel solutions, structural rewrites |

**This is device-native differentiation**: Ring position changes behavior in real-time by adjusting temperature (creativity), top_p (diversity), and repeat_penalty (conciseness).

**Example**: Refactoring the same code at strictness 10 vs. 90 produces visibly different results—from minimal variable fixes to complete functional rewrites.

### Workflow

1. **Copy** code/text to clipboard
2. **Adjust** strictness via Actions Ring (optional)
3. **Press** persona button
4. **Feel** haptic feedback (pulse → confirmation)
5. **Paste** AI-enhanced result

No UI windows, no context switching—pure flow state.

## 📂 Project Structure

```
personakeys/
├── src/
│   ├── Actions/              # Persona implementations
│   │   ├── DebuggerAction.cs
│   │   ├── RefactorAction.cs
│   │   ├── DocumenterAction.cs
│   │   ├── ArchitectAction.cs
│   │   └── StrictnessAdjustmentAction.cs
│   ├── Services/             # Core services
│   │   ├── LLMService.cs
│   │   ├── ClipboardService.cs
│   │   └── SettingsService.cs
│   ├── Models/               # Data structures
│   └── manifest.json         # Plugin metadata
├── CONTRIBUTING.md           # Development guidelines
├── SECURITY.md              # Security best practices
├── ROADMAP.md               # Future development plans
└── LICENSE                  # MIT License
```

## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Architecture overview
- Adding new actions
- Coding standards
- Testing guidelines
- Pull request process

## 🗺️ Roadmap

- **Phase 1** (Current): Developer Persona Pack - 4 core coding personas
- **Phase 2** (Q3 2026): Extensible persona system with community packs
- **Phase 3** (Q4 2026): Context intelligence and multi-step workflows
- **Phase 4** (Q1 2027): Enterprise features and team collaboration

See [ROADMAP.md](ROADMAP.md) for detailed timeline and feature plans.

## 🔒 Security

- Store API keys securely (not in manifest or code)
- Review clipboard data before sending to LLM providers
- Use local models (Ollama) for sensitive code

See [SECURITY.md](SECURITY.md) for vulnerability reporting and security best practices.

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

## 💬 Support & Community

- **Issues**: [GitHub Issues](https://github.com/yourusername/personakeys/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/personakeys/discussions)
- **Documentation**: This README and inline code comments

---

**Built with the [Logitech Actions SDK](https://developer.logi.com/actions-sdk)**
