using System.IO;
using APNestClient.ModLoader;
using HarmonyLib;
using Il2Cpp;

namespace APNestClient;

[HarmonyPatch(typeof(ProgressionManager), "ProgressionSaveRoot", MethodType.Getter)]
public class ProgressionSaveRedirect
{
    static bool Prefix(ref string __result)
    {
        __result = Path.Combine(ModLoaderPaths.DataDirectory, "APNestClient", "ProgressionSave");
        return false;
    }
}
