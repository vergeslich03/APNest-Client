using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace APNestClient.Util;

public static class GraphUtil
{
    public static PunchcardDefinitionV2 GetPunchcardDefinitionById(string id)
    {
        Dictionary<string, PunchcardDefinitionV2> allDefinitions = RequisitionConsoleManager.Instance.AllDefinitions;

        if (allDefinitions.TryGetValue(id, out PunchcardDefinitionV2 exactMatch))
        {
            return exactMatch;
        }

        foreach (KeyValuePair<string, PunchcardDefinitionV2> entry in allDefinitions)
        {
            if (entry.Key.Trim() == id)
            {
                return entry.Value;
            }
        }

        throw new System.Collections.Generic.KeyNotFoundException("No PunchcardDefinitionV2 found for '" + id);
    }
    
    public static T FindFirstNode<T>(PunchcardDefinitionV2 punchcard) where T : Il2CppObjectBase
    {
        foreach (Node node in punchcard.Graph.nodes)
        {
            T match = node.TryCast<T>();
            if (match != null)  return match;
        }
        
        return null;
    }

    public static void SubmitTeleprinterReport(PunchcardDefinitionV2 punchcard, string entityId)
    {
       State_TeleprinterText teleprinterNode =  FindFirstNode<State_TeleprinterText>(punchcard);
       if (teleprinterNode != null)
       {
           string text = teleprinterNode.Text != null ? teleprinterNode.Text.Get() : null;
           if (string.IsNullOrEmpty(text))
           {
               return;
           }
           
           foreach (State_TeleprinterText.StringReplacement replacement in teleprinterNode.EntityIDToReplace)
           {
               text = text.Replace(replacement.Text, entityId);
           }

           Il2CppSystem.Collections.Generic.List<string> lines = FireMissionTokenProcessor.ProcessBlock(text, new Il2CppSystem.Collections.Generic.Dictionary<string, GridReference>());
           Teleprinter printer = Teleprinter.GetTeleprinter(teleprinterNode.Printer);
           printer.SubmitLines(Guid.NewGuid().ToString(), lines.Cast<Il2CppSystem.Collections.Generic.IEnumerable<string>>(), null, teleprinterNode.WaitUntilComplete);

           if (teleprinterNode.AlarmState != Teleprinter.TeleprinterAlarmState.None)
           {
               printer.SignalAlarm(teleprinterNode.AlarmState);
           }
       }
    }
}