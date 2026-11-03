using HarmonyLib;

namespace APNestClient;

// Suppress the game's own Punchcard unlocks, to let Archipelago handle Punchcards.
// Skipping OnEnter also skips the node's hand-off to its following node, which stalls the
// rest of that graph branch (like the mission 3 ammo bunker briefing), so we advance the graph ourselves.
[HarmonyPatch(typeof(State_AddPunchcard), "OnEnter")]
public class SuppressAddPunchcardNode
{
    static bool Prefix(State_AddPunchcard __instance, StateNode.NodeExecutionState state)
    {
        __instance.OnExit(state, "To");
        return false;
    }
}
