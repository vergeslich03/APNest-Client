using System;
using System.Collections.Generic;
using APNestClient.ModLoader;
using Il2Cpp;
using HarmonyLib;

namespace APNestClient;

[HarmonyPatch(typeof(MissionManager), "MarkMissionComplete")]
public class MissionCompleteChecks
{
    public static event Action<string> LocationCompleted;
    private static Dictionary<string, string> _lookupTable = new LookupTables(LookupTables.TableType.MissionLocations).MissionNameToAPLocationNameTable;
    
    static void Postfix(MissionManager __instance)
    {
        var currentMission = __instance.CurrentMission.MissionID;
        Logger.Msg("MissionCompleteCheck: " + currentMission);
        LocationCompleted?.Invoke(_lookupTable[currentMission]);
    }
}