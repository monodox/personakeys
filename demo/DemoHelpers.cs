using System.Text;

namespace PersonaKeys.Demo;

public static class Runner
{
    public static async Task RunPersona(LLMClient llm, Settings settings, Persona persona, string action)
    {
        var input = Console.IsInputRedirected
            ? await Console.In.ReadToEndAsync()
            : PromptMultiline("Paste your code (end with a blank line):");

        if (string.IsNullOrWhiteSpace(input)) { Err("No input provided."); return; }

        Header($"{persona.Name}  ·  {action}");
        Info($"Provider : {settings.ApiProvider}  |  Model: {settings.Model}");
        Info($"Mode     : {settings.StrictnessMode(50)} (temp={settings.Temperature}, topP={settings.TopP}, rp={settings.RepeatPenalty})");
        Console.WriteLine();

        var cts = StartSpinner();
        try
        {
            var result = await llm.SendAsync(persona, input);
            cts.Cancel();
            Console.Write("\r" + new string(' ', 40) + "\r");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(result);
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            cts.Cancel();
            Console.WriteLine();
            Err($"Request failed: {ex.Message}");
        }
    }

    public static void ShowStrictness(int value)
    {
        value = Math.Clamp(value, 0, 100);
        var s = new Settings();
        var (temp, topP, rp) = s.MapStrictness(value);
        var mode = s.StrictnessMode(value);

        Header($"🎛️  Strictness: {value}  →  {mode}");
        Console.WriteLine($"  temperature    = {temp}");
        Console.WriteLine($"  top_p          = {topP}");
        Console.WriteLine($"  repeat_penalty = {rp}");
        Console.WriteLine();

        var filled = value / 5;
        var bar = new string('█', filled) + new string('░', 20 - filled);
        Console.ForegroundColor = value < 40 ? ConsoleColor.Blue : value < 70 ? ConsoleColor.Yellow : ConsoleColor.Magenta;
        Console.WriteLine($"  [{bar}] {value}/100");
        Console.ResetColor();
    }

    public static async Task RunInteractive(LLMClient llm, Settings settings)
    {
        Header("🎛️  PersonaKeys Interactive Demo");
        Info($"Provider: {settings.ApiProvider}  |  Model: {settings.Model}");

        if (settings.ApiProvider == "ollama")
        {
            Console.Write("  Checking Ollama... ");
            var ok = await llm.CheckOllamaAsync();
            Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(ok ? "✓ connected" : $"✗ not found — run: ollama pull {settings.Model}");
            Console.ResetColor();
        }

        var strictness = 50;

        while (true)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ─────────────────────────────────────────");
            Console.ResetColor();
            Console.WriteLine("  [1] Debug    [2] Refactor    [3] Document    [4] Architect");
            Console.WriteLine($"  [s] Strictness ({strictness} · {settings.StrictnessMode(strictness)})    [q] Quit");
            Console.Write("\n  > ");

            var key = Console.ReadLine()?.Trim().ToLowerInvariant();
            Console.WriteLine();

            if (key == "q") break;

            if (key == "s")
            {
                Console.Write("  Enter strictness (0-100): ");
                if (int.TryParse(Console.ReadLine(), out var sv))
                {
                    strictness = Math.Clamp(sv, 0, 100);
                    var (t, p, r) = settings.MapStrictness(strictness);
                    settings.Temperature = t;
                    settings.TopP = p;
                    settings.RepeatPenalty = r;
                    ShowStrictness(strictness);
                }
                continue;
            }

            var persona = key switch
            {
                "1" => Personas.Debugger,
                "2" => Personas.Refactorer,
                "3" => Personas.Documenter,
                "4" => Personas.Architect,
                _   => null
            };

            if (persona == null) { Err("Unknown command — use 1/2/3/4/s/q"); continue; }

            await RunPersona(llm, settings, persona, persona.Name);
        }

        Console.WriteLine("\n  Bye 👋");
    }

    public static void PrintHelp()
    {
        Header("PersonaKeys Demo CLI  —  no device required");
        Console.WriteLine("  Usage:");
        Console.WriteLine("    dotnet run -- debug       < mycode.cs");
        Console.WriteLine("    dotnet run -- refactor    < mycode.cs");
        Console.WriteLine("    dotnet run -- document    < mycode.cs");
        Console.WriteLine("    dotnet run -- architect   < mycode.cs");
        Console.WriteLine("    dotnet run -- strictness 75");
        Console.WriteLine("    dotnet run -- interactive");
        Console.WriteLine();
        Console.WriteLine("  Config: demo-settings.json  (or plugin settings auto-detected)");
        Console.WriteLine("  Default provider: Ollama  (ollama pull llama3.2:latest)");
    }

    private static string PromptMultiline(string prompt)
    {
        Console.WriteLine($"  {prompt}");
        var sb = new StringBuilder();
        string? line;
        while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            sb.AppendLine(line);
        return sb.ToString();
    }

    private static void Header(string text)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n  {text}");
        Console.ResetColor();
    }

    private static void Info(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  {text}");
        Console.ResetColor();
    }

    private static void Err(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✗ {text}");
        Console.ResetColor();
    }

    private static CancellationTokenSource StartSpinner()
    {
        var cts = new CancellationTokenSource();
        Task.Run(async () =>
        {
            var frames = new[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
            var i = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                Console.Write($"\r  {frames[i++ % frames.Length]} Thinking...");
                await Task.Delay(80).ConfigureAwait(false);
            }
        }, cts.Token);
        return cts;
    }
}
