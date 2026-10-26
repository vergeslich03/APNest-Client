using HarmonyLib;

namespace APNestClient;

// The CBT failure path normally kills after the third salvo after 2 state changes. To remedy that, we need to remove the state changes.
[HarmonyPatch(typeof(State_RandomBranch), "OnEnter")]
public class DeathLinkForceDeathBranchHook
{
    public static bool Prefix(State_RandomBranch __instance, StateNode.NodeExecutionState state)
    {
        if (!ShouldForceDeath())
        {
            return true;
        }

        if (!TryGetDeathPortName(__instance, out string deathPort))
        {
            return true;
        }

        __instance.OnExit(state, deathPort);
        return false;
    }

    // fixes a MissionFailed loop (without it the Missionfailed would fire every frame and freeze the game)
    public static bool ShouldForceDeath()
    {
        return APSession.PendingRemoteDeathLink &&
               MissionManager.Instance != null &&
               MissionManager.Instance.CurrentMissionState != null &&
               !MissionManager.Instance.CurrentMissionState.Failed;
    }

    // Get the port that leads to the MissionFailed State (only the CB Timer has it).
    public static bool TryGetDeathPortName(State_RandomBranch branch, out string portName)
    {
        portName = null;
        if (branch == null || branch.To == null)
        {
            return false;
        }

        for (int i = 0; i < branch.To.Length; i++)
        {
            NodePort port = branch.GetOutputPort("To " + i);
            if (port?.Connection?.node?.TryCast<State_MissionFailed>() != null)
            {
                portName = "To " + i;
                return true;
            }
        }

        return false;
    }
}
