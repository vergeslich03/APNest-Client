using System;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using Logger = APNestClient.ModLoader.Logger;
using Object = UnityEngine.Object;

namespace APNestClient;

// Tracks whether a mission is actually running and initialized, so traps don't fire while
// a mission is being torn down or while the next one is still loading
public static class MissionReadyGuard
{
    // grace period after a mission starts, so every scene object has time to get initialized
    private const float GRACE_SECONDS = 5f;

    private static volatile bool _missionActive;
    private static volatile bool _startTimePending;
    private static float _missionStartTime;
    private static int _missionGeneration;
    private static bool _missionChangedSubscribed;

    // increments on every mission (re)start
    public static int MissionGeneration => _missionGeneration;

    public static void RegisterMissionChangedEventHook()
    {
        if (_missionChangedSubscribed || MissionManager.Instance == null)
        {
            return;
        }

        Action<MissionGraph, MissionGraph> handler = (from, to) =>
        {
            if (to != null)
            {
                NotifyMissionStarted();
            }
        };
        MissionManager.Instance.MissionChanged += handler;
        _missionChangedSubscribed = true;
    }

    public static void NotifyMissionStarted()
    {
        Interlocked.Increment(ref _missionGeneration);
        // timestamp is taken lazily on the main thread in IsReady()
        _startTimePending = true;
        _missionActive = true;
    }

    public static void NotifyMissionLeft(string reason)
    {
        if (_missionActive)
        {
            Logger.Msg("Mission no longer active (" + reason + "), holding back traps");
        }
        _missionActive = false;
    }

    // must be called from the main thread
    public static bool IsReady()
    {
        if (!_missionActive)
        {
            return false;
        }

        if (_startTimePending)
        {
            _missionStartTime = Time.realtimeSinceStartup;
            _startTimePending = false;
        }

        if (Time.realtimeSinceStartup - _missionStartTime < GRACE_SECONDS)
        {
            return false;
        }

        if (MissionManager.Instance == null || MissionManager.Instance.CurrentMission == null)
        {
            return false;
        }

        if (FireMission.Instance == null || TurretController.Instance == null)
        {
            return false;
        }

        if (Object.FindFirstObjectByType<DieselEngineController>() == null ||
            Object.FindFirstObjectByType<ShellSlotPool>() == null)
        {
            return false;
        }

        var valves = HighPressureSystemManager.GetAllRegisteredValves();
        return valves != null && valves.Count > 0;
    }
}

[HarmonyPatch(typeof(MissionManager), "MarkMissionComplete")]
public static class MissionReadyGuardMarkCompleteHook
{
    static void Prefix() => MissionReadyGuard.NotifyMissionLeft("MarkMissionComplete");
}

[HarmonyPatch(typeof(MissionManager), "MarkMissionFailed")]
public static class MissionReadyGuardMarkFailedHook
{
    static void Prefix() => MissionReadyGuard.NotifyMissionLeft("MarkMissionFailed");
}

[HarmonyPatch(typeof(MissionManager), "FinishMission")]
public static class MissionReadyGuardFinishHook
{
    static void Prefix() => MissionReadyGuard.NotifyMissionLeft("FinishMission");
}

[HarmonyPatch(typeof(MissionManager), "ReturnToMap")]
public static class MissionReadyGuardReturnToMapHook
{
    static void Prefix() => MissionReadyGuard.NotifyMissionLeft("ReturnToMap");
}

[HarmonyPatch(typeof(MissionManager), "LoadMainMenu")]
public static class MissionReadyGuardMainMenuHook
{
    static void Prefix() => MissionReadyGuard.NotifyMissionLeft("LoadMainMenu");
}

[HarmonyPatch(typeof(MissionManager), "EndOperationAndReturnToMenu")]
public static class MissionReadyGuardEndOperationHook
{
    static void Prefix() => MissionReadyGuard.NotifyMissionLeft("EndOperationAndReturnToMenu");
}

// a restart may not raise MissionChanged, so treat it as a fresh mission explicitly
[HarmonyPatch(typeof(MissionManager), "ReloadCurrentMission")]
public static class MissionReadyGuardReloadHook
{
    static void Postfix() => MissionReadyGuard.NotifyMissionStarted();
}
