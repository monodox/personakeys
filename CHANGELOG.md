# Changelog

All notable changes to PersonaKeys will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- **Ollama Integration Improvements**:
  - Switched from `/api/generate` to `/api/chat` for structured messages
  - Updated default model to `llama3.2:latest`
  - Added model availability checking on plugin startup
  - Improved error messages with actionable instructions
- **Enhanced Ring Parameterization**:
  - Ring now controls temperature (0.1-0.9), top_p (0.7-0.95), and repeat_penalty (1.05-1.2)
  - Multi-parameter modulation for finer behavioral control
- **Tightened Persona Prompts**:
  - Reduced verbosity with explicit output format constraints
  - Added stop tokens (`\n\n---\n\n`, `<END>`) to prevent runaway generation
  - Lowered max tokens per persona (800-1200) for concise responses
- **Documentation Updates**:
  - Emphasized on-device inference capability in README
  - Added LLM integration details to CONTRIBUTING.md
  - Updated configuration examples with new parameters

### Planned
- Custom persona creation UI
- Persona pack marketplace
- Context memory across sessions
- Multi-platform clipboard support
- Custom icon editor
- Persona presets and templates
- Export/import persona configurations

## [1.0.0] - 2026-02-12

### Added
- Initial release of PersonaKeys
- **Developer Persona Pack** with 4 specialized AI operators:
  - **Debugger**: Root cause analysis and fix suggestions
  - **Refactorer**: Code optimization with strictness-aware output
  - **Documenter**: Inline comment and documentation generation
  - **Architect**: Design pattern suggestions and architectural guidance
- **Actions Ring Strictness Control**: 0-100 mapping to multiple AI parameters
  - Ring controls: temperature (creativity), top_p (diversity), repeat_penalty (conciseness)
  - 0-20 → Strict mode (T=0.1, P=0.7, RP=1.05)
  - 20-40 → Conservative mode (T=0.3, P=0.8, RP=1.08)
  - 40-60 → Balanced mode (T=0.5, P=0.85, RP=1.1)
  - 60-80 → Creative mode (T=0.7, P=0.9, RP=1.15)
  - 80-100 → Experimental mode (T=0.9, P=0.95, RP=1.2)
- **Haptic Feedback Patterns**:
  - Ring turn → Tactile tick
  - AI invoked → Pulse
  - Success → Confirmation vibration
  - Error → Distinct error vibration
- **Clipboard Workflow**: Seamless copy-process-paste integration
- **Multi-Provider LLM Support**:
  - Ollama (local, recommended)
  - OpenAI (GPT-4, GPT-3.5)
  - Azure OpenAI
  - Anthropic Claude
- **Settings Management**: Persistent configuration storage
- **Settings Action**: Display current plugin configuration
- **Base Architecture**: Service-based design (LLM, Clipboard, Settings)
- **Windows Clipboard Integration**: Native P/Invoke implementation
- **Error Handling**: Comprehensive try-catch with user feedback
- **Logging**: SDK-integrated logging system
- **Documentation**:
  - Comprehensive README
  - Contributing guidelines
  - Security policy
  - Code of conduct
  - MIT License

### Technical Details
- **Platform**: .NET 8 / C#
- **SDK**: Logitech Actions SDK
- **Architecture**: Plugin with service layer abstraction
- **Supported Devices**:
  - Logitech MX Creative Console
  - Loupedeck CT
  - Loupedeck Live
  - Loupedeck Live S
  - Razer Stream Controller

### Developer Experience
- Hot reload support with `dotnet watch build`
- Clean separation of concerns
- XML documentation comments
- NuGet package management
- Visual Studio / VS Code / Rider support

## Version History

### [1.0.0] - 2026-02-12
*Initial public release*

---

## Categories

Changes are grouped into these categories:

- **Added**: New features
- **Changed**: Changes in existing functionality
- **Deprecated**: Soon-to-be removed features
- **Removed**: Now removed features
- **Fixed**: Bug fixes
- **Security**: Vulnerability fixes (marked with `[SECURITY]`)

## Links

- [Unreleased]: https://github.com/yourusername/personakeys/compare/v1.0.0...HEAD
- [1.0.0]: https://github.com/yourusername/personakeys/releases/tag/v1.0.0
