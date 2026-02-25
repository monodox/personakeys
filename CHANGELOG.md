# Changelog

All notable changes to PersonaKeys will be documented in this file.

Format: [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
Versioning: [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

---

## [Unreleased]

### Planned
- Interactive settings UI (form inputs, not just display)
- Custom persona creation via JSON/YAML
- Persona pack marketplace
- Session memory across interactions
- Direct IDE integration (VS Code, JetBrains)
- macOS clipboard support
- Custom icon editor

---

## [1.0.0] - 2026-02-25

### Added

**Core plugin**
- `PersonaKeysPlugin` — main plugin entry point extending `Loupedeck.Plugin`
- `PluginLog` helper wrapping `PluginLogFile` for SDK-integrated logging
- Startup Ollama model availability check (non-blocking, writes hint to clipboard if model missing)

**Developer Persona Pack — 4 actions**
- `DebuggerAction` — root cause analysis + minimal fix from clipboard code/stack trace
- `RefactorAction` — code quality and readability improvements
- `DocumenterAction` — inline comments and docstrings (JSDoc, XML, Python)
- `ArchitectAction` — design pattern suggestions with trade-off comparison

**Strictness dial**
- `StrictnessAdjustmentAction` — `PluginDynamicAdjustment` mapping 0–100 ring position to three LLM parameters simultaneously
  - 0–20 Strict: temperature=0.1, topP=0.70, repeatPenalty=1.05
  - 20–40 Conservative: temperature=0.3, topP=0.80, repeatPenalty=1.08
  - 40–60 Balanced: temperature=0.5, topP=0.85, repeatPenalty=1.10
  - 60–80 Creative: temperature=0.7, topP=0.90, repeatPenalty=1.15
  - 80–100 Experimental: temperature=0.9, topP=0.95, repeatPenalty=1.20
- Reset button returns to Balanced (50)
- Dial label shows current mode and value in real time

**Settings**
- `SettingsAction` — logs current configuration on press
- `SettingsService` — JSON persistence at `%LOCALAPPDATA%\Logi\LogiPluginService\PluginData\PersonaKeys\personakeys-settings.json`

**LLM service**
- `LLMService` — multi-provider HTTP client
  - Ollama via `/api/chat` (structured system/user messages)
  - OpenAI Chat Completions API
  - Azure OpenAI
  - Anthropic Messages API
- Model availability check for Ollama on startup
- Configurable timeout (default 60s)

**Clipboard service**
- `ClipboardService` — Windows clipboard via P/Invoke (`user32.dll`, `kernel32.dll`)
- Unicode read/write with retry-safe open/close pattern

**Build system**
- `PersonaKeys.csproj` referencing `PluginApi.dll` from `C:\Program Files\Logi\LogiPluginService\`
- PostBuild target writes `.link` file to Logi Plugin Service plugins directory
- PostBuild target sends `loupedeck:plugin/PersonaKeys/reload` for hot reload
- `System.Text.Json` 9.0.0 (patched from vulnerable 8.0.0)

### Fixed
- Replaced `Logi.PluginCore` NuGet reference (does not exist on nuget.org) with direct `PluginApi.dll` reference from installed Logi Plugin Service
- Fixed `SetClipboardData` P/Invoke return type comparison
- Removed `BasePlugin`/`GetPluginSetting` dependency from `SettingsService` (not available in SDK)
- Corrected `PluginDynamicCommand` and `PluginDynamicAdjustment` constructor signatures (positional, no named params)

### Security
- [SECURITY] Upgraded `System.Text.Json` from 8.0.0 to 9.0.0 (fixes GHSA-8g4q-xg66-9fp4, GHSA-hh2w-p6rv-4g7w)

---

## Categories

- Added — new features
- Changed — changes in existing functionality
- Deprecated — soon-to-be removed features
- Removed — removed features
- Fixed — bug fixes
- Security — vulnerability fixes (tagged `[SECURITY]`)

---

[Unreleased]: https://github.com/monodox/personakeys/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/monodox/personakeys/releases/tag/v1.0.0
