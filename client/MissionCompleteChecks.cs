using System;
using System.Collections.Generic;
using APNestClient.Data;
using APNestClient.ModLoader;
using HarmonyLib;

namespace APNestClient;

[HarmonyPatch(typeof(MissionManager), "MarkMissionComplete")]
public class MissionCompleteChecks
{
    public static event Action<string> LocationCompleted;
    private static IReadOnlyDictionary<string, MissionInfo> _missions = DataHolders.MissionsByGameId;
    
    static void Postfix(MissionManager __instance)
    {
        var currentMission = __instance.CurrentMission.MissionID;
        Logger.Msg("MissionCompleteCheck: " + currentMission);
        LocationCompleted?.Invoke(_missions[currentMission].APName);
    }
}