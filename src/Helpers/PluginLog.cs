using Loupedeck;

namespace PersonaKeys;

internal static class PluginLog
{
    private static PluginLogFile? _log;

    public static void Init(PluginLogFile log) => _log = log;

    public static void Info(string text) => _log?.Info(text);
    public static void Warning(string text) => _log?.Warning(text);
    public static void Error(string text) => _log?.Error(text);
}
