using System;
using System.Collections.Generic;
using Il2Cpp;
using HarmonyLib;
using MelonLoader;

namespace APNestClient;

[HarmonyPatch(typeof(MissionManager), "MarkMissionComplete")]
public class MissionCompleteChecks
{
    public static event Action<string> LocationCompleted;
    
    static void Postfix(MissionManager __instance)
    {
        var currentMission = __instance.CurrentMission.MissionID;
        MelonLogger.Msg("MissionCompleteCheck: " + currentMission);
        Dictionary<string, string> lookupTable = new LookupTables(LookupTables.TableType.Locations).MissionNameToAPLocationNameTable;
        LocationCompleted?.Invoke(lookupTable[currentMission]);
    }
}