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
    private volatile bool _pendingMissionLoad;
    private volatile string _pendingMissionId;

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
        switch (trapName)
        {
            case "TrapFillMagazine":
            {
                PunchcardDefinitionV2 starShellCard =  RequisitionConsoleManager.Instance.AllDefinitions["STARShell"];
                ShellDefinition shell = null;
                foreach (Node node in starShellCard.Graph.nodes)
                {
                    State_AddShell shellCandidate = node.TryCast<State_AddShell>();
                    if (shellCandidate != null)
                    {
                        shell = shellCandidate.Shell;
                        break;
                    }
                }

                if (shell == null)
                {
                    MelonLogger.Warning("Shell configured for Trap not found (STARShell)");
                    break;
                }
                
                ShellSlotPool shellSlotPool = UnityEngine.Object.FindFirstObjectByType<ShellSlotPool>();
                try
                {
                    bool inserted = true;
                    while (inserted)
                    {
                        inserted = shellSlotPool.InsertShell(
                            shell,
                            ShellSlotPool.ShellInsertionMode.FillOneThenNext,
                            ShellSlotPool.ShellSource.Punchcard,
                            out _,
                            out _
                        );
                    }
                }
                catch (Il2CppInterop.Runtime.Il2CppException)
                {
                    // game throws Error if the whole magazine is full instead of returning false
                    // Not sure if this is a bug or simply forgotten and handled via try/catch in game
                    MelonLogger.Msg("Trap Magazine Fill completed");
                }

                break;
            }
        }
    }

    public void RegisterMissionChangedEventHook()
    {
        if (_missionChangedSubscribed || MissionManager.Instance == null)
        {
            return;
        }

        Action<MissionGraph, MissionGraph> handler = (from, to) =>
        {
            if (to != null)
            {
                _pendingMissionId = to.MissionID;
                _pendingMissionLoad = true;
            }
        };
        MissionManager.Instance.MissionChanged += handler;
        _missionChangedSubscribed = true;
    }
    
    public void ProcessPendingMissionLoad()
    {
        if (!_pendingMissionLoad)
        {
            return;
        }

        _pendingMissionLoad = false;
        DrainQueue();
    }
}