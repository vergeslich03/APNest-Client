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
    private static Dictionary<string, string> _lookupTable = new LookupTables(LookupTables.TableType.MissionLocations).MissionNameToAPLocationNameTable;
    
    static void Postfix(MissionManager __instance)
    {
        var currentMission = __instance.CurrentMission.MissionID;
        MelonLogger.Msg("MissionCompleteCheck: " + currentMission);
        LocationCompleted?.Invoke(_lookupTable[currentMission]);
    }
}