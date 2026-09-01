using System.IO;
using HarmonyLib;
using Il2Cpp;
using MelonLoader.Utils;

namespace APNestClient;

[HarmonyPatch(typeof(ProgressionManager), "ProgressionSaveRoot", MethodType.Getter)]
public class ProgressionSaveRedirect
{
    static bool Prefix(ref string __result)
    {
        __result = Path.Combine(MelonEnvironment.UserDataDirectory, "APNestClient", "ProgressionSave");
        return false;
    }
}
