using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppSleepyNodes;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;

namespace APNestClient;

// Was a Harmony patch on MissionManager.LoadMission (private, default param) — that
// crashed the game hard at mod-init with no log/exception at all, because Harmony
// tried to detour a method with no real native IL2CPP function pointer. Switched to
// subscribing directly to MissionManager's own public C# MissionChanged event
// instead — no native detour involved, so it can't hit that failure mode.
public static class ObjectiveGraphScan
{
    private static bool _hooked;
    private static bool _scanned;

    public static void TryHook()
    {
        if (_hooked) return;
        if (MissionManager.Instance == null) return;

        MissionManager.Instance.MissionChanged += new System.Action<MissionGraph, MissionGraph>((oldMission, newMission) => RunScan());
        _hooked = true;
    }

    private static string PunchcardIds(Il2CppSystem.Collections.Generic.List<PunchcardDefinitionV2> cards)
    {
        if (cards == null) return "";
        var ids = new List<string>();
        foreach (var card in cards)
        {
            if (card == null) continue;
            ids.Add(card.ID);
        }
        return string.Join(",", ids);
    }

    private static void RunScan()
    {
        if (_scanned) return;
        _scanned = true;

        var allGraphs = Resources.FindObjectsOfTypeAll<MissionGraph>();
        MelonLogger.Msg("ObjectiveGraphScan: scanning " + allGraphs.Length + " loaded MissionGraph assets for State_Objective nodes");

        var missionList = new List<string>();
        foreach (var graph in allGraphs)
        {
            if (graph == null) continue;
            if (string.IsNullOrEmpty(graph.MissionID)) continue;
            string nameRaw = graph.MissionName != null ? graph.MissionName.Raw : "<null>";
            string nameKey = graph.MissionName != null ? graph.MissionName.Key : "<null>";
            string required = PunchcardIds(graph.RequiredPunchcards);
            string unlocked = PunchcardIds(graph.UnlockedPunchcards);
            missionList.Add(graph.MissionID + " | graphName=" + graph.name + " | MissionName.Raw=" + nameRaw + " | MissionName.Key=" + nameKey + " | RequiredPunchcards=[" + required + "] | UnlockedPunchcards=[" + unlocked + "]");
        }
        missionList.Sort(StringComparer.Ordinal);
        MelonLogger.Msg("ObjectiveGraphScan: MISSION CATALOG (" + missionList.Count + " missions)");
        foreach (var line in missionList)
            MelonLogger.Msg("ObjectiveGraphScan: MISSION " + line);

        foreach (var graph in allGraphs)
        {
            if (graph == null || graph.nodes == null) continue;

            var hits = new List<string>();
            foreach (var node in graph.nodes)
            {
                if (node == null) continue;
                var classPtr = IL2CPP.il2cpp_object_get_class(node.Pointer);
                var typeName = IL2CPP.il2cpp_class_get_name_(classPtr);
                if (typeName == "State_Objective")
                {
                    var stateNode = node.TryCast<StateNode>();
                    hits.Add(stateNode != null ? stateNode.NodeID : "<unknown>");
                }
            }

            if (hits.Count > 0)
            {
                MelonLogger.Msg("ObjectiveGraphScan: HIT graph=" + graph.name + " missionId=" + graph.MissionID + " objectiveNodeIds=" + string.Join(",", hits));
            }
        }

        MelonLogger.Msg("ObjectiveGraphScan: done");
    }
}
