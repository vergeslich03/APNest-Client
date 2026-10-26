using System;
using HarmonyLib;
using Logger = APNestClient.ModLoader.Logger;

namespace APNestClient;

[HarmonyPatch(typeof(MissionManager), nameof(MissionManager.MarkMissionFailed))]
public class DeathLinkSendHook
{
    public static event Action MissionFailed;

    // MarkMissionFailed can be hit more than once for the same failure, so it needs to ignore the second onwards, so no DL echoes are created
    public static void Prefix(MissionManager __instance, out bool __state)
    {
        __state = __instance.CurrentMissionState != null && __instance.CurrentMissionState.Failed;
    }

    public static void Postfix(MissionManager __instance, bool __state)
    {
        // consume the remote flag first, so it can't get stuck if the mission was already failed
        if (APSession.PendingRemoteDeathLink)
        {
            APSession.PendingRemoteDeathLink = false;
            return;
        }

        if (__state)
        {
            Logger.Msg("[DeathLink] MarkMissionFailed called on an already failed mission, ignoring.");
            return;
        }

        MissionFailed?.Invoke();
    }
}
