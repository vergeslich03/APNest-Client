using System;
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
        MelonLogger.Msg("MissionCompleteChecks: " + currentMission);
        var apName = GetAPName(currentMission);
        LocationCompleted?.Invoke(apName);
    }

    private static string GetAPName(string missionName)
    {
        switch (missionName)
        {
            case "Hospital False Flag":
                return "Mission 1: Calibration Fire";
            case "ceremony and HCHE":
                return "Mission 2: Fire and Light";
            case "Insurrections and Requisitions":
                return "Mission 3: Liberation";
            case "Artillery Introduction":
                return "Mission 4: Counter-Battery";
            case "IronRoad":
                return "Mission 5: Iron Road";
            case "SiegeOfCartagena":
                return "Mission 6: Siege of Cartagena";
            case "The Gorge":
                return "Mission 7: The Gorge";
            case "RockofGibraltar":
                return "Mission 8: Rock of Gibraltar";
            case "DeadReckoning":
                return "Mission 9: Dead Reckoning";
            case "FireOnCall":
                return "Mission 10: Fire on Call";
            case "HighTide":
                return "Mission 11: High Tide";
            case "BlindFire":
                return "Mission 12: Blind Fire";
            case "PhantomBattery":
                return "Mission 13: Phantom Battery";
            case "FinalHarvest":
                return "Mission 14: Final Harvest";
            case "WhiteShells":
                return "Mission 15: White Shells";
        }
        
        return missionName;
    }
}