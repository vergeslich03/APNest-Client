using System.IO;

#if BEPINEX
using BepInEx;
#else
using MelonLoader.Utils;
#endif

namespace APNestClient.ModLoader;

public class ModLoaderPaths
{
#if BEPINEX
    public static string DataDirectory => Path.Combine(Paths.BepInExRootPath, "APNestClient");
#else
    public static string DataDirectory => Path.Combine(MelonEnvironment.UserDataDirectory, "APNestClient");
#endif
}