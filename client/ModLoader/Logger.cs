#if BEPINEX
using BepInEx.Logging;
#else
using MelonLoader;
#endif

namespace APNestClient.ModLoader;

internal static class Logger
{
#if BEPINEX
    private static ManualLogSource _log;

    internal static void Init(ManualLogSource log) => _log = log;
    
    public static void Msg(string msg) => _log?.LogInfo(msg);
    public static void Warning(string msg) => _log?.LogWarning(msg);
    public static void Error(string msg) => _log?.LogError(msg);
#else
    public static void Msg(string msg) => MelonLogger.Msg(msg);
    public static void Warning(string msg) => MelonLogger.Warning(msg);
    public static void Error(string msg) => MelonLogger.Error(msg);
#endif
}