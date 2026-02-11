using System.Net.Http;
using System.Text;
using System.Text.Json;
using PersonaKeys.Models;
using Logi.PluginCore;

namespace PersonaKeys.Services;

/// <summary>
/// Service for communicating with LLM providers (Ollama, OpenAI, Azure, Anthropic)
/// </summary>
public class LLMService : IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly HttpClient _httpClient;

    public LLMService(SettingsService settingsService)
    {
        _settingsService = settingsService;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(2)
        };
    }

    /// <summary>
    /// Sends a request to the configured LLM provider
    /// </summary>
    public async Task<LLMResponse> SendRequestAsync(LLMRequest request)
    {
        var settings = _settingsService.GetSettings();

        try
        {
            return settings.ApiProvider.ToLowerInvariant() switch
            {
                "ollama" => await SendOllamaRequestAsync(request, settings),
                "openai" => await SendOpenAIRequestAsync(request, settings),
                "azure" => await SendAzureOpenAIRequestAsync(request, settings),
                "anthropic" => await SendAnthropicRequestAsync(request, settings),
                _ => new LLMResponse 
                { 
                    Success = false, 
                    Error = $"Unknown API provider: {settings.ApiProvider}" 
                }
            };
        }
        catch (Exception ex)
        {
            Log.Error($"LLM request failed: {ex.Message}");
            return new LLMResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Check if the configured Ollama model is available
    /// </summary>
    public async Task<bool> CheckOllamaModelAsync()
    {
        try
        {
            var settings = _settingsService.GetSettings();
            var endpoint = $"{settings.ApiEndpoint}/api/tags";
            
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return false;
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            
            if (jsonDoc.RootElement.TryGetProperty("models", out var models))
            {
                foreach (var model in models.EnumerateArray())
                {
                    if (model.TryGetProperty("name", out var name))
                    {
                        var modelName = name.GetString() ?? string.Empty;
                        if (modelName.StartsWith(settings.Model))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private async Task<LLMResponse> SendOllamaRequestAsync(LLMRequest request, PluginSettings settings)
    {
        // Use /api/chat for structured messages (better for persona prompting)
        var endpoint = $"{settings.ApiEndpoint}/api/chat";
        
        var payload = new
        {
            model = settings.Model,
            messages = new[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            },
            stream = false,
            options = new
            {
                temperature = request.Temperature,
                top_p = request.TopP,
                repeat_penalty = request.RepeatPenalty,
                num_predict = request.MaxTokens,
                stop = new[] { "\n\n---\n\n", "<END>" }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(endpoint, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            if (errorBody.Contains("model") && errorBody.Contains("not found"))
            {
                return new LLMResponse
                {
                    Success = false,
                    Error = $"Model '{settings.Model}' not found. Run: ollama pull {settings.Model}"
                };
            }
            response.EnsureSuccessStatusCode();
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseBody);
        
        var responseText = jsonDoc.RootElement
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;

        return new LLMResponse
        {
            Success = true,
            Content = responseText.Trim()
        };
    }

    private async Task<LLMResponse> SendOpenAIRequestAsync(LLMRequest request, PluginSettings settings)
    {
        var endpoint = string.IsNullOrEmpty(settings.ApiEndpoint) 
            ? "https://api.openai.com/v1/chat/completions"
            : settings.ApiEndpoint;

        var payload = new
        {
            model = settings.Model,
            messages = new[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            },
            temperature = request.Temperature,
            max_tokens = request.MaxTokens
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        };

        httpRequest.Headers.Add("Authorization", $"Bearer {settings.ApiKey}");

        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseBody);
        
        var responseText = jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;

        var tokensUsed = jsonDoc.RootElement
            .GetProperty("usage")
            .GetProperty("total_tokens")
            .GetInt32();

        return new LLMResponse
        {
            Success = true,
            Content = responseText.Trim(),
            TokensUsed = tokensUsed
        };
    }

    private async Task<LLMResponse> SendAzureOpenAIRequestAsync(LLMRequest request, PluginSettings settings)
    {
        // Azure OpenAI uses similar format to OpenAI
        var payload = new
        {
            messages = new[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            },
            temperature = request.Temperature,
            max_tokens = request.MaxTokens
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, settings.ApiEndpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        };

        httpRequest.Headers.Add("api-key", settings.ApiKey);

        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseBody);
        
        var responseText = jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;

        return new LLMResponse
        {
            Success = true,
            Content = responseText.Trim()
        };
    }

    private async Task<LLMResponse> SendAnthropicRequestAsync(LLMRequest request, PluginSettings settings)
    {
        var endpoint = string.IsNullOrEmpty(settings.ApiEndpoint)
            ? "https://api.anthropic.com/v1/messages"
            : settings.ApiEndpoint;

        var payload = new
        {
            model = settings.Model,
            system = request.SystemPrompt,
            messages = new[]
            {
                new { role = "user", content = request.UserPrompt }
            },
            temperature = request.Temperature,
            max_tokens = request.MaxTokens
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        };

        httpRequest.Headers.Add("x-api-key", settings.ApiKey);
        httpRequest.Headers.Add("anthropic-version", "2023-06-01");

        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseBody);
        
        var responseText = jsonDoc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;

        return new LLMResponse
        {
            Success = true,
            Content = responseText.Trim()
        };
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
