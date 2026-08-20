using HarmonyLib;
using Il2CppSleepyNodes;

namespace APNestClient;

// Suppress the game's own Punchcard unlocks, to let Archipelago handle Punchcards
[HarmonyPatch(typeof(State_AddPunchcard), "OnEnter")]
public class SuppressAddPunchcardNode
{
    static bool Prefix()
    {
        return false;
    }
}
