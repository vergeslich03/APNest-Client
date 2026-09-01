using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppSleepyNodes;
using MelonLoader;

namespace APNestClient;

// Hard-locks mission entry until the AP punchcard items the mission's apworld region
// requires have actually been received.
[HarmonyPatch]
public class MissionEntryGuard
{
    private static readonly Dictionary<string, List<string>> _prerequisites =
        new LookupTables(LookupTables.TableType.MissionPrerequisites).MissionPrerequisiteTable;

    private static readonly Dictionary<string, string> _gameIdToApName = BuildReverseItemTable();

    private static Dictionary<string, string> BuildReverseItemTable()
    {
        Dictionary<string, string> reverse = new();
        foreach (KeyValuePair<string, string> entry in
                 new LookupTables(LookupTables.TableType.Items).ApItemNameToGameIdTable)
        {
            reverse[entry.Value] = entry.Key;
        }

        return reverse;
    }

    public static List<string> MissingRequirements(string missionId)
    {
        List<string> missing = new();

        if (missionId == null)
        {
            return missing;
        }

        if (!_prerequisites.TryGetValue(missionId, out List<string> requiredCards))
        {
            return missing;
        }

        if (ProgressionManager.Instance == null)
        {
            return missing;
        }

        foreach (string cardId in requiredCards)
        {
            if (!ProgressionManager.Instance.IsCardUnlocked(cardId))
            {
                missing.Add(_gameIdToApName.TryGetValue(cardId, out string apName) ? apName : cardId);
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

        MelonLogger.Msg("[MissionEntryGuard] Blocked entry to '" + mission.MissionID
                        + "' - missing item(s): " + string.Join(", ", missing));
        return false;
    }
}
