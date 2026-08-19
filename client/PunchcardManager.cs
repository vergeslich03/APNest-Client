using Il2CppSleepyNodes;
using HarmonyLib;

namespace APNestClient;

[HarmonyPatch(typeof(StateNode), "OnEnter")]
public class PunchcardManager
{
    static void Postfix(StateNode __instance)
    {
        // TODO unlocking of Punchcards
    }
}