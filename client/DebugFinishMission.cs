using HarmonyLib;
using UnityEngine.InputSystem;
using Logger = APNestClient.ModLoader.Logger;

namespace APNestClient;

// Debug helper: finishes the running mission immediately with every medal category at Gold.
// FinishMission resolves each category's Bronze/Silver/Gold MedalConditionSet against the tracked
// values and writes the result into CurrentMissionState.Medals, so forcing Resolve() to true for the
// duration of that call lets the game itself award (and save) Gold. The normal MissionCompleteChecks
// and MedalAchievedChecks hooks then send the AP locations as usual.
public static class DebugFinishMission
{
    private const Key FINISH_KEY = Key.F10;

    private static bool _forceAllMedals;

    public static bool ForceAllMedals => _forceAllMedals;

    // must be called from the main thread
    public static void PollHotkey()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard[FINISH_KEY].wasPressedThisFrame)
        {
            return;
        }

        Run();
    }

    public static void Run()
    {
        MissionManager missionManager = MissionManager.Instance;
        if (missionManager == null || missionManager.CurrentMission == null || missionManager.CurrentMissionState == null)
        {
            Logger.Warning("[Debug] No mission running, nothing to finish.");
            return;
        }

        if (missionManager.CurrentMissionState.Failed)
        {
            Logger.Warning("[Debug] Current mission is already failed, not finishing it.");
            return;
        }

        Logger.Msg("[Debug] Finishing mission '" + missionManager.CurrentMission.MissionID + "' with all Gold medals.");

        _forceAllMedals = true;
        try
        {
            if (!missionManager.CurrentMissionState.Complete)
            {
                missionManager.MarkMissionComplete(true);
            }
            missionManager.FinishMission();
        }
        finally
        {
            _forceAllMedals = false;
        }
    }
}

[HarmonyPatch(typeof(MedalConditionSet), nameof(MedalConditionSet.Resolve))]
public static class DebugForceMedalConditionHook
{
    static bool Prefix(ref bool __result)
    {
        if (!DebugFinishMission.ForceAllMedals)
        {
            return true;
        }

        __result = true;
        return false;
    }
}
