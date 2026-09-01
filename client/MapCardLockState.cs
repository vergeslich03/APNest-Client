using HarmonyLib;
using Il2Cpp;
using Il2CppSleepyNodes;

namespace APNestClient;

// Renders a gated mission's map card as locked, should the mission card for some reason be visible.
[HarmonyPatch(typeof(MapCard), "Init")]
public class MapCardLockState
{
    static void Postfix(MapCard __instance, MissionGraph mission)
    {
        if (mission == null)
        {
            return;
        }

        if (MissionEntryGuard.MissingRequirements(mission.MissionID).Count == 0)
        {
            return;
        }

        if (__instance.OnState_NotUnlocked != null)
        {
            __instance.OnState_NotUnlocked.Invoke();
        }
    }
}
