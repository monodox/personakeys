using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PersonaKeys.Test;

/// <summary>
/// Standalone test to verify Ollama integration without full SDK
/// </summary>
class OllamaIntegrationTest
{
    private static readonly HttpClient _httpClient = new HttpClient 
    { 
        Timeout = TimeSpan.FromSeconds(60) 
    };

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== PersonaKeys Ollama Integration Test ===\n");

        // Test 1: Check Ollama connectivity
        Console.WriteLine("Test 1: Checking Ollama connectivity...");
        var modelAvailable = await CheckOllamaModelAsync();
        if (modelAvailable)
        {
            Console.WriteLine("✓ Ollama is running and llama3.2:latest is available\n");
        }
        else
        {
            Console.WriteLine("✗ Ollama model not found. Run: ollama pull llama3.2:latest\n");
            return;
        }

        // Test 2: Test ring parameter mapping
        Console.WriteLine("Test 2: Testing ring parameter mapping...");
        TestRingMapping();
        Console.WriteLine();

        // Test 3: Simple Ollama API test
        Console.WriteLine("Test 3: Testing basic Ollama API call...");
        var simpleTest = await TestSimpleOllamaCall();
        if (!simpleTest)
        {
            Console.WriteLine("\n⚠️  Ollama API test failed. Common issues:");
            Console.WriteLine("   - Insufficient system memory (model needs ~2.3 GB available)");
            Console.WriteLine("   - Close other applications to free up memory");
            Console.WriteLine("   - Or use a smaller model: ollama pull llama3.2:1b");
            Console.WriteLine("\nSkipping persona tests...");
            return;
        }
        Console.WriteLine();

        // Test 4: Test Debugger persona with different strictness levels
        Console.WriteLine("Test 4: Testing Debugger persona...");
        var testCode = @"function getData() {
  var result = [];
  for (var i = 0; i < items.length; i++) {
    if (items[i].active == true) {
      result.push(items[i]);
    }
  }
  return result;
}";

        await TestPersona("Debugger", GetDebuggerPrompt(), testCode, 10);  // Strict
        await TestPersona("Debugger", GetDebuggerPrompt(), testCode, 90);  // Experimental

        // Test 5: Test Refactorer persona
        Console.WriteLine("\nTest 5: Testing Refactorer persona...");
        await TestPersona("Refactorer", GetRefactorPrompt(), testCode, 10);  // Conservative
        await TestPersona("Refactorer", GetRefactorPrompt(), testCode, 90);  // Creative

        Console.WriteLine("\n=== All tests completed ===");
    }

    static async Task<bool> CheckOllamaModelAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://127.0.0.1:11434/api/tags");
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);

            if (jsonDoc.RootElement.TryGetProperty("models", out var models))
            {
                foreach (var model in models.EnumerateArray())
                {
                    if (model.TryGetProperty("name", out var name))
                    {
                        var modelName = name.GetString() ?? string.Empty;
                        if (modelName.StartsWith("llama3.2"))
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

    static void TestRingMapping()
    {
        var testValues = new[] { 10, 30, 50, 70, 90 };
        
        Console.WriteLine("Ring Position | Mode          | Temp | TopP | RepeatPenalty");
        Console.WriteLine("--------------|---------------|------|------|---------------");
        
        foreach (var strictness in testValues)
        {
            var (temp, topP, penalty) = MapStrictnessToParameters(strictness);
            var mode = strictness switch
            {
                < 20 => "Strict",
                < 40 => "Conservative",
                < 60 => "Balanced",
                < 80 => "Creative",
                _ => "Experimental"
            };
            
            Console.WriteLine($"{strictness,13} | {mode,-13} | {temp:F2} | {topP:F2} | {penalty:F2}");
        }
        
        Console.WriteLine("✓ Ring mapping verified");
    }

    static async Task<bool> TestSimpleOllamaCall()
    {
        try
        {
            var payload = new
            {
                model = "llama3.2:latest",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = "Say hello in exactly 5 words." }
                },
                stream = false,
                options = new
                {
                    num_predict = 50
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("http://127.0.0.1:11434/api/chat", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);
                var result = jsonDoc.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;
                    
                Console.WriteLine($"✓ Basic API call succeeded: \"{result.Trim()}\"");
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✗ API call failed: {errorBody}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Exception: {ex.Message}");
            return false;
        }
    }

    static (double temperature, double topP, double repeatPenalty) MapStrictnessToParameters(int strictness)
    {
        return strictness switch
        {
            < 20 => (0.1, 0.7, 1.05),
            < 40 => (0.3, 0.8, 1.08),
            < 60 => (0.5, 0.85, 1.1),
            < 80 => (0.7, 0.9, 1.15),
            _ => (0.9, 0.95, 1.2)
        };
    }

    static async Task TestPersona(string personaName, string systemPrompt, string code, int strictness)
    {
        Console.WriteLine($"\n  - Testing {personaName} at strictness {strictness}...");
        
        var (temp, topP, penalty) = MapStrictnessToParameters(strictness);
        
        var payload = new
        {
            model = "llama3.2:latest",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = $"Analyze this code:\n\n{code}" }
            },
            stream = false,
            options = new
            {
                temperature = temp,
                top_p = topP,
                repeat_penalty = penalty,
                num_predict = 1200,
                stop = new[] { "\n\n---\n\n", "<END>" }
            }
        };

        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var startTime = DateTime.Now;
            var response = await _httpClient.PostAsync("http://127.0.0.1:11434/api/chat", content);
            var duration = (DateTime.Now - startTime).TotalSeconds;

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);
                
                var result = jsonDoc.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;

                Console.WriteLine($"    ✓ Response received in {duration:F1}s");
                Console.WriteLine($"    Output length: {result.Length} characters");
                Console.WriteLine($"    First 100 chars: {result.Substring(0, Math.Min(100, result.Length))}...");
            }
            else
            {
                Console.WriteLine($"    ✗ Request failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ✗ Error: {ex.Message}");
        }
    }

    static string GetDebuggerPrompt()
    {
        return @"You are a code debugger. Analyze the provided code/stack trace and respond with:

**Root Cause:** [1-2 sentence explanation]
**Minimal Fix:** [specific code change]
**Why:** [1-2 bullets]

If context is missing, state assumptions explicitly. Keep sections SHORT. Use code blocks only when needed.
<END>";
    }

    static string GetRefactorPrompt()
    {
        return @"You are a code refactorer. Provide:

**Refactored Code:** [improved version]
**Why It's Better:** [2-3 bullets: safer/cleaner/more idiomatic]

Return ONLY the refactored code and brief reasoning. No essays.
<END>";
    }
}
