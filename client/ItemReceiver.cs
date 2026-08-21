using System;
using System.Collections.Concurrent;
using Il2Cpp;
using Il2CppSleepyNodes;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace APNestClient;

public class ItemReceiver
{
    private LookupTables _lookupTable;
    
    private ConcurrentQueue<string> _itemQueue = new();
    private bool _missionChangedSubscribed;

    public ItemReceiver()
    {
        _lookupTable = new LookupTables(LookupTables.TableType.Items);
        APSession.ItemReceived += itemName => ProcessAPItem(itemName);
    }

    private void DrainQueue()
    {
        while (_itemQueue.TryDequeue(out string itemName))
        {
            ProcessAPItem(itemName);
        }
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
        catch (NullReferenceException)
        {
            _itemQueue.Enqueue(apItemName);
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
        switch (spawnName)
        {
            case "SpawnPowderCharges":
            {
                Random rand = new();
                PowderChargeInventory.Instance.AddCharges(rand.Next(5, 26));
                break;
            }
        }
    }

    private void HandleTrapItem(string trapName)
    {
        
    }

    public void RegisterMissionChangedEventHook()
    {
        if (_missionChangedSubscribed || MissionManager.Instance == null)
        {
            return;
        }

        Action<MissionGraph, MissionGraph> handler = (from, to) => DrainQueue();
        MissionManager.Instance.MissionChanged += handler;
        _missionChangedSubscribed = true;
    }
}