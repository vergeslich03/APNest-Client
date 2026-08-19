using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MelonLoader;

[assembly: MelonInfo(typeof(APNestClient.ModMain), "AP Nest Client", "0.0.1", "vergeslichlp")]

namespace APNestClient
{
    public class ModMain : MelonMod
    {
        private List<Type> _allTypes;
        private Dictionary<string, Type> _byFullName;
        
        private ConnectUI _connectUI;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("APNest Client loaded.");
            CollectTypes();
            DumpSleepyNodes();
            DumpDeepInfo();
            
            this._connectUI = new ConnectUI();
        }

        public override void OnGUI()
        {
            
        }

        public override void OnUpdate()
        {
            ObjectiveGraphScan.TryHook();
        }

        private void CollectTypes()
        {
            _allTypes = new List<Type>();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch { types = Array.Empty<Type>(); }
                _allTypes.AddRange(types);
            }

            _byFullName = new Dictionary<string, Type>();
            foreach (Type t in _allTypes)
            {
                if (t.FullName != null && !_byFullName.ContainsKey(t.FullName))
                    _byFullName[t.FullName] = t;
            }
        }

        private static readonly (string FileName, string[] Keywords)[] NameKeywordScans = new (string, string[])[]
        {
            ("mission_scene_candidates.txt", new[] { "Mission", "SceneTemplate" }),
            ("spawn_candidates.txt", new[] { "Arty", "Artillery", "CounterBattery", "Spawn", "Enemy" }),
            ("punchcard_candidates.txt", new[] { "Punchcard", "Requisition", "Card" }),
            ("reward_candidates.txt", new[] { "Reward", "Result", "Unlock", "Outcome" }),
            ("requirement_candidates.txt", new[] { "Requirement" }),
        };

        private void DumpSleepyNodes()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            List<string> nodeLines = new List<string>();
            List<string>[] scanLines = new List<string>[NameKeywordScans.Length];
            for (int i = 0; i < scanLines.Length; i++)
                scanLines[i] = new List<string>();

            foreach (Type t in _allTypes)
            {
                if (t.Namespace == "Il2CppSleepyNodes")
                {
                    string baseName = t.BaseType != null ? t.BaseType.Name : "null";
                    nodeLines.Add(t.Name + " : base=" + baseName);
                }

                for (int i = 0; i < NameKeywordScans.Length; i++)
                {
                    foreach (string keyword in NameKeywordScans[i].Keywords)
                    {
                        if (t.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            scanLines[i].Add(t.FullName);
                            break;
                        }
                    }
                }
            }

            nodeLines.Sort();
            File.WriteAllLines(Path.Combine(baseDir, "sleepynodes_dump.txt"), nodeLines.ToArray());

            for (int i = 0; i < NameKeywordScans.Length; i++)
            {
                scanLines[i].Sort();
                File.WriteAllLines(Path.Combine(baseDir, NameKeywordScans[i].FileName), scanLines[i].ToArray());
            }
        }

        private void DumpDeepInfo()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            List<string> outLines = new List<string>();

            string[] targets = new string[]
            {
                "Il2CppSleepyNodes.Node",
                "Il2CppSleepyNodes.StateNode",
                "Il2CppSleepyNodes.StateNodeEntry",
                "Il2CppSleepyNodes.StateGraph",
                "Il2CppSleepyNodes.ObjectiveEntry",
                "Il2CppSleepyNodes.EventNode",
                "Il2CppSleepyNodes.State_Objective",
                "Il2CppSleepyNodes.ObjectiveStateNode",
                "Il2CppSleepyNodes.ObjectiveResultNode",
                "Il2CppSleepyNodes.ObjectiveGraph",
                "Il2CppSleepyNodes.ObjectiveGraph+ObjectiveResults",
                "Il2CppSleepyNodes.ObjectiveResults",
                "Il2Cpp.ObjectiveResults",
                "Il2Cpp.MedalTrackedValue",
                "Il2CppSleepyNodes.MedalTrackedValue",
                "Il2Cpp.MissionManager+MedalTrackedValue",
                "Il2CppSleepyNodes.State_SpawnMapEntity",
                "Il2CppSleepyNodes.State_MissionFailed",
                "Il2CppSleepyNodes.MissionGraph",
                "Il2CppSleepyNodes.MissionGraph+MissionTypes",
                "Il2Cpp.MissionManager",
                "Il2Cpp.MissionManager+MissionState",
                "Il2Cpp.MissionGraphNotificationListener",
                "Il2Cpp.MissionAdvanceRelay",
                "Il2Cpp.MissionAdvanceReaction",
                "Il2Cpp.MissionImporter",
                "Il2Cpp.MissionImporter+MissionDefinition",
                "Il2Cpp.OperationState",
                "Il2Cpp.OperationState+MissionState",
                "Il2Cpp.MissionDebugLauncher",
                "Il2Cpp.CounterBatteryTimer",
                "Il2Cpp.EntityRoles",
                "Il2Cpp.EntityLocation",
                "Il2Cpp.MapEntityStates",
                "Il2Cpp.RequisitionSlot",
                "Il2Cpp.PunchcardRuntime",
                "Il2Cpp.PunchcardDefinitionV2",
                "Il2Cpp.UserProgression",
                "Il2Cpp.UserProgression+UserCardState",
                "Il2CppSleepyNodes.PunchcardGraph",
                "Il2CppSleepyNodes.State_AddPunchcard",
                "Il2Cpp.RequirementSet",
                "Il2Cpp.RequirementSet+RequirementPair",
                "Il2Cpp.Requirement",
                "Il2Cpp.Requirement+RequirementTypes",
            };

            foreach (string targetName in targets)
            {
                outLines.Add("==== " + targetName + " ====");
                Type t;
                if (_byFullName == null || !_byFullName.TryGetValue(targetName, out t))
                {
                    outLines.Add("  NOT FOUND");
                    outLines.Add("");
                    continue;
                }

                outLines.Add("  IsEnum=" + t.IsEnum + " BaseType=" + (t.BaseType != null ? t.BaseType.FullName : "null"));

                if (t.IsEnum)
                {
                    string[] names = Enum.GetNames(t);
                    for (int i = 0; i < names.Length; i++)
                        outLines.Add("  enum: " + names[i]);
                }

                MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (MethodInfo m in methods)
                {
                    if (m.IsSpecialName)
                        continue;

                    ParameterInfo[] ps = m.GetParameters();
                    string paramList = "";
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (i > 0) paramList += ", ";
                        paramList += ps[i].ParameterType.Name + " " + ps[i].Name;
                    }
                    outLines.Add("  method: " + m.ReturnType.Name + " " + m.Name + "(" + paramList + ")");
                }

                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (FieldInfo f in fields)
                {
                    outLines.Add("  field: " + f.FieldType.Name + " " + f.Name);
                }

                PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo p in properties)
                {
                    string access = (p.CanRead ? "get" : "") + (p.CanRead && p.CanWrite ? "/" : "") + (p.CanWrite ? "set" : "");
                    outLines.Add("  property: " + p.PropertyType.Name + " " + p.Name + " {" + access + "}");
                }

                outLines.Add("");
            }

            string[] fallbackNames = new string[]
            {
                "ObjectiveResults", "MedalTrackedValue",
                "CounterBatteryTimer", "EntityRoles", "EntityLocation", "MapEntityStates",
                "RequisitionSlot", "PunchcardRuntime", "RequirementSet",
            };

            outLines.Add("==== name-based fallback search: " + string.Join(" / ", fallbackNames) + " ====");
            foreach (Type t in _allTypes)
            {
                if (Array.IndexOf(fallbackNames, t.Name) >= 0)
                {
                    outLines.Add("  FOUND: " + t.FullName + " IsEnum=" + t.IsEnum);
                    if (t.IsEnum)
                    {
                        string[] names = Enum.GetNames(t);
                        for (int i = 0; i < names.Length; i++)
                            outLines.Add("    enum: " + names[i]);
                    }
                }
            }
            outLines.Add("");

            File.WriteAllLines(Path.Combine(baseDir, "deep_dump.txt"), outLines.ToArray());
            MelonLogger.Msg("Deep dump written: " + outLines.Count + " lines.");
        }
    }
}
