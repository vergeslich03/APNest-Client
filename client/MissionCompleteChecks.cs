using Il2Cpp;
using HarmonyLib;
using MelonLoader;

namespace APNestClient;

[HarmonyPatch(typeof(MissionManager), "MarkMissionComplete")]
public class MissionCompleteChecks
{
    static void Postfix(MissionManager __instance)
    {
        var currentMission = __instance.CurrentMission;
        MelonLogger.Msg("MissionCompleteChecks: " + currentMission.MissionID);
    }
}