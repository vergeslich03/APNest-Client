using System.Collections.Generic;
using System.Reflection;
using APNestClient.Data;
using APNestClient.ModLoader;
using HarmonyLib;

namespace APNestClient;

// Hard-locks mission entry until the AP punchcard items the mission's apworld region
// requires have actually been received.
[HarmonyPatch]
public class MissionEntryGuard
{
    private static readonly IReadOnlyDictionary<string, MissionInfo> _missions = DataHolders.MissionsByGameId;
    private static readonly IReadOnlyDictionary<string, ItemInfo> _items = DataHolders.ItemsByGameId;

    public static List<string> MissingRequirements(string missionId)
    {
        List<string> missing = new();

        if (missionId == null)
        {
            return missing;
        }

        if (ProgressionManager.Instance == null)
        {
            return missing;
        }
        
        foreach (string cardId in _missions[missionId].Prerequisites)
        {
            if (!ProgressionManager.Instance.IsCardUnlocked(cardId))
            {
                missing.Add(_items[cardId].APName);
            }
        }

        return missing;
    }

    static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (MethodInfo method in typeof(MissionManager).GetMethods(
                     BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (method.Name == "StartOperation")
            {
                yield return method;
            }
        }
    }

    static bool Prefix(MissionGraph mission)
    {
        if (mission == null)
        {
            return true;
        }

        List<string> missing = MissingRequirements(mission.MissionID);
        if (missing.Count == 0)
        {
            return true;
        }

        Logger.Msg("[MissionEntryGuard] Blocked entry to '" + mission.MissionID
                        + "' - missing item(s): " + string.Join(", ", missing));
        return false;
    }
}
