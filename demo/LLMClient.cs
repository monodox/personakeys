using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PersonaKeys.Demo;

public class LLMClient : IDisposable
{
    private readonly Settings _settings;
    private readonly HttpClient _http;

    public LLMClient(Settings settings)
    {
        _settings = settings;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(settings.Timeout) };
    }

    public async Task<string> SendAsync(Persona persona, string input, double? temp = null, double? topP = null, double? rp = null)
    {
        var userPrompt = string.Format(persona.UserPromptTemplate, input);
        var t  = temp  ?? _settings.Temperature;
        var p  = topP  ?? _settings.TopP;
        var r  = rp    ?? _settings.RepeatPenalty;

        return _settings.ApiProvider.ToLowerInvariant() switch
        {
            "ollama"    => await OllamaAsync(persona.SystemPrompt, userPrompt, t, p, r, persona.MaxTokens),
            "openai"    => await OpenAIAsync(persona.SystemPrompt, userPrompt, t, persona.MaxTokens),
            "azure"     => await AzureAsync(persona.SystemPrompt, userPrompt, t, persona.MaxTokens),
            "anthropic" => await AnthropicAsync(persona.SystemPrompt, userPrompt, t, persona.MaxTokens),
            _ => throw new InvalidOperationException($"Unknown provider: {_settings.ApiProvider}")
        };
    }

    public async Task<bool> CheckOllamaAsync()
    {
        try
        {
            var res = await _http.GetAsync($"{_settings.ApiEndpoint}/api/tags");
            if (!res.IsSuccessStatusCode) return false;
            var body = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("models", out var models))
                foreach (var m in models.EnumerateArray())
                    if (m.TryGetProperty("name", out var n) && (n.GetString() ?? "").StartsWith(_settings.Model))
                        return true;
            return false;
        }
        catch { return false; }
    }

    private async Task<string> OllamaAsync(string system, string user, double temp, double topP, double rp, int maxTokens)
    {
        var payload = new
        {
            model = _settings.Model,
            messages = new[] { new { role = "system", content = system }, new { role = "user", content = user } },
            stream = false,
            options = new { temperature = temp, top_p = topP, repeat_penalty = rp, num_predict = maxTokens }
        };
        var res = await PostJsonAsync($"{_settings.ApiEndpoint}/api/chat", payload);
        return JsonDocument.Parse(res).RootElement.GetProperty("message").GetProperty("content").GetString()?.Trim() ?? "";
    }

    private async Task<string> OpenAIAsync(string system, string user, double temp, int maxTokens)
    {
        var endpoint = string.IsNullOrEmpty(_settings.ApiEndpoint) ? "https://api.openai.com/v1/chat/completions" : _settings.ApiEndpoint;
        var payload = new { model = _settings.Model, messages = new[] { new { role = "system", content = system }, new { role = "user", content = user } }, temperature = temp, max_tokens = maxTokens };
        var req = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = JsonContent(payload) };
        req.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
        var res = await (await _http.SendAsync(req)).Content.ReadAsStringAsync();
        return JsonDocument.Parse(res).RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? "";
    }

    private async Task<string> AzureAsync(string system, string user, double temp, int maxTokens)
    {
        var payload = new { messages = new[] { new { role = "system", content = system }, new { role = "user", content = user } }, temperature = temp, max_tokens = maxTokens };
        var req = new HttpRequestMessage(HttpMethod.Post, _settings.ApiEndpoint) { Content = JsonContent(payload) };
        req.Headers.Add("api-key", _settings.ApiKey);
        var res = await (await _http.SendAsync(req)).Content.ReadAsStringAsync();
        return JsonDocument.Parse(res).RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()?.Trim() ?? "";
    }

    private async Task<string> AnthropicAsync(string system, string user, double temp, int maxTokens)
    {
        var endpoint = string.IsNullOrEmpty(_settings.ApiEndpoint) ? "https://api.anthropic.com/v1/messages" : _settings.ApiEndpoint;
        var payload = new { model = _settings.Model, system, messages = new[] { new { role = "user", content = user } }, temperature = temp, max_tokens = maxTokens };
        var req = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = JsonContent(payload) };
        req.Headers.Add("x-api-key", _settings.ApiKey);
        req.Headers.Add("anthropic-version", "2023-06-01");
        var res = await (await _http.SendAsync(req)).Content.ReadAsStringAsync();
        return JsonDocument.Parse(res).RootElement.GetProperty("content")[0].GetProperty("text").GetString()?.Trim() ?? "";
    }

    private async Task<string> PostJsonAsync(string url, object payload)
    {
        var res = await _http.PostAsync(url, JsonContent(payload));
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsStringAsync();
    }

    private static StringContent JsonContent(object obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    public void Dispose() => _http.Dispose();
}
