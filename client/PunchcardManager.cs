using Il2CppSleepyNodes;
using Il2CppInterop.Runtime;
using HarmonyLib;
using MelonLoader;

namespace APNestClient;

[HarmonyPatch(typeof(StateNode), "OnEnter")]
public class PunchcardManager
{
    static void Postfix(StateNode __instance)
    {
        var classPtr = IL2CPP.il2cpp_object_get_class(__instance.Pointer);
        var nodeType = IL2CPP.il2cpp_class_get_name_(classPtr);

        string nodeId = __instance.NodeID;

        string graphType = "null";
        string graphName = "null";
        var graph = __instance.graph;
        if (graph != null)
        {
            var graphClassPtr = IL2CPP.il2cpp_object_get_class(graph.Pointer);
            graphType = IL2CPP.il2cpp_class_get_name_(graphClassPtr);
            graphName = graph.name;
        }

        MelonLogger.Msg("Node: " + nodeType + " id=" + nodeId + " graph=" + graphType + " (" + graphName + ")");
    }
}