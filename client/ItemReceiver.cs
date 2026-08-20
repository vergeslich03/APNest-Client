using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace APNestClient;

public class ItemReceiver
{
    private LookupTables _lookupTable;
    
    public ItemReceiver()
    {
        _lookupTable = new LookupTables(LookupTables.TableType.Items);
        APSession.ItemReceived += itemName => ProcessAPItem(itemName);
    }

    public void ProcessAPItem(string apItemName)
    {
        try
        {
            string itemName = _lookupTable.ApItemNameToGameId[apItemName];
            
            if (itemName.StartsWith("Spawn"))
            {
                HandleSpawnItem(itemName);
                return;
            }
            if (itemName.StartsWith("Trap"))
            {
                HandleTrapItem(itemName);
                return;
            }
        
            HandlePunchcardItem(itemName);
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            MelonLogger.Error("Unknown Item '" + apItemName + "'");
            return;
        }
    }

    private void HandlePunchcardItem(string punchcardName)
    {
        MelonLogger.Msg("Processing Punchcard: " + punchcardName);
        
        // guard against duplicate punchcards
        if (ProgressionManager.Instance.IsCardUnlocked(punchcardName))
        {
            MelonLogger.Msg("Punchcard already unlocked");
            return;
        }
        
        PunchcardDefinitionV2 punchcard = RequisitionConsoleManager.Instance.AllDefinitions[punchcardName];
        List<PunchcardDefinitionV2> punchcardList = new();
        punchcardList.Add(punchcard);

        PunchcardUnlockGuard.AllowNextUnlock = true;
        try
        {
            ProgressionManager.Instance.UnlockPunchcards(punchcardList.Cast<IEnumerable<PunchcardDefinitionV2>>());
        }
        finally
        {
            PunchcardUnlockGuard.AllowNextUnlock = false;
        }
        ProgressionManager.Instance.SaveProgression();
        RequisitionConsoleManager.Instance.AddNewCardsToDeck(punchcardList);
        
        MelonLogger.Msg("Added Punchcard: " + punchcardName);
    }

    private void HandleSpawnItem(string spawnName)
    {
        
    }

    private void HandleTrapItem(string trapName)
    {
        
    }
}