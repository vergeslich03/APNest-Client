using HarmonyLib;

namespace APNestClient;

// The CBT failure path fires a salvo every 30s, that's too long for my liking, so I remove the timer to kill with the first.
[HarmonyPatch(typeof(Event_TimeInterval), "ShouldRun")]
public class DeathLinkSkipDeathTimerHook
{
    public static bool Prefix(Event_TimeInterval __instance, ref bool __result)
    {
        if (!DeathLinkForceDeathBranchHook.ShouldForceDeath())
        {
            return true;
        }

        // waits for the set time to expire to give you a chance to read the Death-Link Message
        CounterBatteryTimer timer = CounterBatteryTimer.Instance;
        if (timer == null || timer.TimeRemaining > 0f)
        {
            return true;
        }

        if (!IsCbtDeathInterval(__instance))
        {
            return true;
        }

        __result = true;
        return false;
    }

    private static bool IsCbtDeathInterval(Event_TimeInterval interval)
    {
        State_ConditionBranch condition = interval.GetOutputPort("To")?.Connection?.node?.TryCast<State_ConditionBranch>();
        if (condition == null)
        {
            return false;
        }

        State_RandomBranch salvoBranch = condition.GetOutputPort("To")?.Connection?.node?.TryCast<State_RandomBranch>();
        return DeathLinkForceDeathBranchHook.TryGetDeathPortName(salvoBranch, out _);
    }
}
