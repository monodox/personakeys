using System.Text;
using PersonaKeys.Demo;

Console.OutputEncoding = Encoding.UTF8;

var command = args.Length > 0 ? args[0].ToLowerInvariant() : "interactive";
var settings = Settings.Load();
var llm = new LLMClient(settings);

switch (command)
{
    case "debug":       await Runner.RunPersona(llm, settings, Personas.Debugger,   "Debugging");   break;
    case "refactor":    await Runner.RunPersona(llm, settings, Personas.Refactorer, "Refactoring"); break;
    case "document":    await Runner.RunPersona(llm, settings, Personas.Documenter, "Documenting"); break;
    case "architect":   await Runner.RunPersona(llm, settings, Personas.Architect,  "Analyzing");   break;
    case "strictness":  Runner.ShowStrictness(args.Length > 1 && int.TryParse(args[1], out var v) ? v : 50); break;
    case "interactive": await Runner.RunInteractive(llm, settings); break;
    default:            Runner.PrintHelp(); break;
}
