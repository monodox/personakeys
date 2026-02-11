using System.Text.Json.Serialization;

namespace PersonaKeys.Models;

/// <summary>
/// Represents a PersonaKeys command with its configuration.
/// 
/// PERSONA PACK ARCHITECTURE:
/// PersonaKeys uses an extensible persona system. The Developer Persona Pack
/// includes 4 specialized coding assistants. Future persona packs can target
/// writers, students, marketers, designers, and other use cases.
/// </summary>
public class PersonaCommand
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("systemPrompt")]
    public string SystemPrompt { get; set; } = string.Empty;

    [JsonPropertyName("requiresInput")]
    public bool RequiresInput { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("personaPack")]
    public string PersonaPack { get; set; } = "developer"; // developer, writer, student, marketer, designer
}

/// <summary>
/// Settings for PersonaKeys plugin
/// </summary>
public class PluginSettings
{
    [JsonPropertyName("apiProvider")]
    public string ApiProvider { get; set; } = "ollama"; // ollama, openai, azure, anthropic

    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("apiEndpoint")]
    public string ApiEndpoint { get; set; } = "http://localhost:11434"; // Default Ollama

    [JsonPropertyName("model")]
    public string Model { get; set; } = "llama3.2:latest";

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.7;

    [JsonPropertyName("topP")]
    public double TopP { get; set; } = 0.9;

    [JsonPropertyName("repeatPenalty")]
    public double RepeatPenalty { get; set; } = 1.1;

    [JsonPropertyName("maxTokens")]
    public int MaxTokens { get; set; } = 2048;

    [JsonPropertyName("enableHaptics")]
    public bool EnableHaptics { get; set; } = true;

    [JsonPropertyName("timeout")]
    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>
/// Represents an LLM request with ring-driven parameters
/// </summary>
public class LLMRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserPrompt { get; set; } = string.Empty;
    public string? Context { get; set; }
    
    // Ring-driven parameters for real-time behavioral modulation
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 0.9;
    public double RepeatPenalty { get; set; } = 1.1;
    public int MaxTokens { get; set; } = 2048;
}

/// <summary>
/// Represents an LLM response
/// </summary>
public class LLMResponse
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Error { get; set; }
    public int TokensUsed { get; set; }
}
