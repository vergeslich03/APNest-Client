using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using Il2CppSleepyNodes;

namespace APNestClient;

[HarmonyPatch(typeof(MissionManager), "FinishMission")]
public class MedalAchievedChecks
{
    public static event Action<string> LocationCompleted;
    private static Dictionary<string, string> _lookupTable = new LookupTables(LookupTables.TableType.MedalLocations).MedalNameToAPLocationNameTable;
    
    static void Prefix(MissionManager __instance,
        out (MissionGraph mission, MissionManager.MissionState state) __state)
    {
        __state = (__instance.CurrentMission, __instance.CurrentMissionState);
    }

    static void Postfix((MissionGraph mission, MissionManager.MissionState state) __state)
    {
        foreach (var medal in __state.state.Medals)
        {
            for (var type = 1; type <= medal.Value; type++)
            {
                MedalTier tier = (MedalTier)type;
                string medalTierId = __state.mission.MissionID + "-" + medal.Key + "-" + tier;
                LocationCompleted?.Invoke(_lookupTable[medalTierId]);
            }
        }
    }
}