using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace APNestClient;

// Blocks the "start mission" button if it is for some reason accessible.
// Unlocks when required Items have been acquired
[HarmonyPatch(typeof(MissionInfoDisplay), "ActivateMission")]
public class MissionInfoDeployGuard
{
    static bool Prefix(MissionInfoDisplay __instance)
    {
        MapCard card = __instance.SourceCard;
        if (card == null || card.Mission == null)
        {
            return true;
        }

        List<string> missing = MissionEntryGuard.MissingRequirements(card.Mission.MissionID);
        if (missing.Count == 0)
        {
            return true;
        }

        MelonLogger.Msg("[MissionInfoDeployGuard] Deploy blocked for '" + card.Mission.MissionID
                        + "' - missing item(s): " + string.Join(", ", missing));
        return false;
    }
}
