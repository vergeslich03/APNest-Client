using HarmonyLib;
using Il2Cpp;

namespace APNestClient;

// make sure only the clients calls go through, so AP handles unlocks
[HarmonyPatch(typeof(ProgressionManager), "UnlockPunchcards")]
public class PunchcardUnlockGuard
{
    public static bool AllowNextUnlock;

    static bool Prefix(ref Il2CppSystem.Collections.Generic.List<string> __result)
    {
        if (AllowNextUnlock)
        {
            return true;
        }

        __result = new Il2CppSystem.Collections.Generic.List<string>();
        return false;
    }
}
