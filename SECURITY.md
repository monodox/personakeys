# Security Policy

## Supported Versions

We release patches for security vulnerabilities for the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of PersonaKeys seriously. If you discover a security vulnerability, please follow these steps:

### 1. **Do Not** Open a Public Issue

Please do not report security vulnerabilities through public GitHub issues, discussions, or pull requests.

### 2. Report Privately

Send a detailed report to: **[your-email@example.com]**

Include:
- Type of vulnerability
- Full paths of source file(s) related to the vulnerability
- Location of the affected source code (tag/branch/commit or direct URL)
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the vulnerability, including how an attacker might exploit it

### 3. What to Expect

- **Acknowledgment**: We will acknowledge receipt of your vulnerability report within 48 hours
- **Updates**: We will provide regular updates (at least every 5 business days) on our progress
- **Timeline**: We aim to release a fix within 90 days of the initial report
- **Credit**: We will credit you in the security advisory (unless you prefer to remain anonymous)

### 4. Disclosure Policy

- Please give us reasonable time to address the vulnerability before any public disclosure
- We will coordinate with you on the disclosure timeline
- We will publish a security advisory when the fix is released

## Security Best Practices

### For Users

1. **API Key Security**
   - Never commit API keys to version control
   - Use environment variables or secure credential storage
   - Rotate API keys regularly
   - Use separate keys for development and production

2. **Keep Updated**
   - Always use the latest stable version
   - Subscribe to GitHub releases for security updates
   - Review changelog for security-related fixes

3. **Local Model Usage**
   - For maximum security, use Ollama (local model)
   - No API keys required
   - All processing happens on your machine
   - No data leaves your device

4. **Settings Configuration**
   - Review plugin settings regularly
   - Use the minimum necessary permissions
   - Audit clipboard access patterns

### For Contributors

1. **Code Security**
   - Never commit secrets, API keys, or credentials
   - Use `.gitignore` to exclude sensitive files
   - Review code for injection vulnerabilities
   - Validate all user input
   - Use parameterized queries where applicable

2. **Dependencies**
   - Keep dependencies up to date
   - Review security advisories for dependencies
   - Use `dotnet list package --vulnerable` to check for vulnerabilities
   - Minimize external dependencies

3. **HTTP Requests**
   - Always use HTTPS for external API calls
   - Validate SSL/TLS certificates
   - Implement proper timeout mechanisms
   - Handle errors securely (don't leak sensitive info)

4. **Clipboard Handling**
   - Be mindful of sensitive data in clipboard
   - Clear clipboard when appropriate
   - Don't log clipboard contents
   - Implement clipboard access responsibly

## Known Security Considerations

### Clipboard Access

PersonaKeys reads from and writes to the system clipboard. Users should be aware:
- The plugin temporarily processes clipboard content
- Content is sent to configured LLM provider (local or cloud)
- No clipboard history is stored by PersonaKeys
- Clear clipboard after processing sensitive code if needed

### LLM Provider Security

When using cloud LLM providers:
- Your code is sent to external services (OpenAI, Azure, Anthropic)
- Review the privacy policies of your chosen provider
- Consider using local models (Ollama) for sensitive code
- Avoid processing code with proprietary or confidential information via cloud APIs

### API Key Storage

- API keys are stored in plugin settings (local machine)
- Settings are stored by Logitech Plugin Service
- On Windows: `C:\Users\USERNAME\AppData\Local\Logi\LogiPluginService\`
- Keys are not encrypted by default
- Consider using Windows Credential Manager for production use

## Security Updates

Security updates will be released in the following ways:

1. **Patch Releases** - Critical security fixes (e.g., 1.0.1, 1.0.2)
2. **GitHub Security Advisories** - Official vulnerability announcements
3. **Release Notes** - CHANGELOG.md will tag security fixes with `[SECURITY]`

Subscribe to GitHub releases to be notified of security updates.

## Scope

This security policy applies to:
- PersonaKeys plugin code in this repository
- Official releases and distributions
- Documented APIs and interfaces

Out of scope:
- Third-party LLM providers (OpenAI, Azure, Anthropic, Ollama)
- Logitech Options+ / Loupedeck software
- Operating system clipboard implementations
- User's development environment

## Hall of Fame

We'd like to thank the following individuals for responsibly disclosing security vulnerabilities:

*(None yet - be the first!)*

---

**Last Updated**: February 12, 2026
