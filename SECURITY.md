# Security Policy

## Supported Versions

| Version | Supported |
|---------|-----------|
| 1.0.x   | ✅ Yes    |
| < 1.0   | ❌ No     |

## Reporting a Vulnerability

Please do not report security vulnerabilities through public GitHub issues.

Send a private report to: **security@personakeys.dev** (or open a [GitHub Security Advisory](https://github.com/monodox/personakeys/security/advisories/new))

Include:
- Type of vulnerability
- Affected file(s) and location (tag/branch/commit)
- Steps to reproduce
- Proof-of-concept or exploit code if available
- Impact assessment

### What to expect

- Acknowledgment within 48 hours
- Status updates every 5 business days
- Fix target within 90 days of report
- Credit in the security advisory (unless you prefer anonymity)

Please give us reasonable time to address the issue before any public disclosure.

---

## Security Best Practices

### API Keys

- Never commit API keys to version control
- Use environment variables or a secrets manager
- Rotate keys regularly and use separate keys per environment
- For maximum security, use Ollama (local) — no keys required, nothing leaves your machine

### Settings file

PersonaKeys stores settings at:
`%LOCALAPPDATA%\Logi\LogiPluginService\PluginData\PersonaKeys\personakeys-settings.json`

This file may contain API keys. It is stored on your local machine only and is not encrypted by default. Restrict file system access to this path if you are on a shared machine.

### Clipboard

PersonaKeys reads from and writes to the system clipboard. Be aware:
- Clipboard content is sent to your configured LLM provider (local or cloud)
- No clipboard history is stored by PersonaKeys
- Avoid copying code containing secrets, credentials, or PII before invoking a persona
- Use Ollama for sensitive or proprietary code — cloud providers receive your clipboard content

### Cloud LLM providers

When using OpenAI, Azure OpenAI, or Anthropic:
- Your clipboard content is transmitted to external services
- Review the privacy policy of your chosen provider
- Do not process code containing secrets, tokens, or confidential business logic via cloud APIs

### HTTP requests

- All external API calls use HTTPS
- Timeouts are enforced (default 60 seconds)
- Errors are logged locally and never expose raw API responses to the clipboard

### For contributors

- Never commit secrets, API keys, or credentials
- Run `dotnet list package --vulnerable` before submitting a PR
- Keep dependencies up to date
- Validate all user input
- Do not log clipboard contents

---

## Known Considerations

### System.Text.Json

Versions below 9.0.0 have known high-severity vulnerabilities (GHSA-8g4q-xg66-9fp4, GHSA-hh2w-p6rv-4g7w). PersonaKeys uses 9.0.0+.

### Dependency check

```powershell
cd src
dotnet list package --vulnerable
```

---

## Security Updates

Security fixes are released as patch versions (e.g., 1.0.1) and tagged `[SECURITY]` in [CHANGELOG.md](CHANGELOG.md). Subscribe to GitHub releases to be notified.

---

Last updated: February 2026
