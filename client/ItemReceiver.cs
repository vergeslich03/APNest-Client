using System;
using System.Collections.Concurrent;
using System.IO;
using APNestClient.Data;
using APNestClient.Util;
using Il2CppInterop.Runtime;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;
using Logger = APNestClient.ModLoader.Logger;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace APNestClient;

public class ItemReceiver
{
    private const float CBT_DEFAULT_TIME = 300f;
    private const string CB_DEFAULT_ID = "APArtillery";
    private const int VALVE_SABOTAGE_MIN = 2;
    private const int VALVE_SABOTAGE_MAX = 5;

    // only keeps a few entries for traps, that can only be triggered once per mission
    // originally debugging for Issue #22, but good nonetheless as to not make it too repetitive
    // I imagine starting every mission with closing valves and starting the engine gets old fast
    private const int ONCE_PER_MISSION_TRAP_QUEUE_LIMIT = 3;
    private static readonly System.Collections.Generic.HashSet<string> OncePerMissionTraps = new() { "TrapSabotage" };
    
    
    private string _itemQueueFile;
    private object _itemQueueLock = new();

    private IReadOnlyDictionary<string, ItemInfo> _items;

    private ConcurrentQueue<string> _itemQueue = new();
    private int _drainedMissionGeneration;

    private bool _sabotageThisMission = false;

    // while draining, re-queued items are only written to disk once at the end of the drain to conserve resources
    private bool _draining;
    private int _droppedDuringDrain;

    public ItemReceiver()
    {
        _itemQueueFile = Path.Combine(APSession.DataDirectory, "ItemQueue.txt");

        System.Collections.Generic.IReadOnlyDictionary<string, ItemInfo> _items = DataHolders.ItemsByApName;
        APSession.ItemReceived += itemName => ProcessAPItem(itemName);
        APSession.SeedChanged += ClearQueue;

        if (!File.Exists(_itemQueueFile))
        {
            File.Create(_itemQueueFile).Close();
        }

        int dropped = 0;
        foreach (string queuedItem in File.ReadAllLines(_itemQueueFile))
        {
            if (!TryEnqueue(queuedItem))
            {
                dropped++;
            }
        }

        if (dropped > 0)
        {
            Logger.Warning("Dropped " + dropped + " surplus once-per-mission traps from the Item Queue");
            File.WriteAllLines(_itemQueueFile, _itemQueue);
        }
    }

    // enqueues the item unless it is a once-per-mission trap that is already queued often enough
    private bool TryEnqueue(string apItemName)
    {
        if (_items.ContainsKey(apItemName))
        {
            string gameId = _items[apItemName].GameId;

            if (OncePerMissionTraps.Contains(gameId))
            {
                int alreadyQueued = 0;
                foreach (string queuedItem in _itemQueue)
                {
                    if (queuedItem == apItemName)
                    {
                        alreadyQueued++;
                    }
                }

                if (alreadyQueued >= ONCE_PER_MISSION_TRAP_QUEUE_LIMIT)
                {
                    return false;
                }   
            }
        }

        _itemQueue.Enqueue(apItemName);
        return true;
    }

    private void ClearQueue()
    {
        lock (_itemQueueLock)
        {
            _itemQueue.Clear();
            File.WriteAllLines(_itemQueueFile, _itemQueue);
        }

        Logger.Msg("Item Queue cleared for new seed.");
    }

    private void DrainQueue()
    {
        // only process what was queued before this drain started. Items that cannot be handled yet
        // are re-enqueued by ProcessAPItem, and must wait for the next mission instead of being dequeued again in the same loop.
        int itemsToProcess = _itemQueue.Count;
        _draining = true;
        _droppedDuringDrain = 0;
        try
        {
            for (int i = 0; i < itemsToProcess && _itemQueue.TryDequeue(out string itemName); i++)
            {
                ProcessAPItem(itemName);
            }
        }
        finally
        {
            _draining = false;
        }

        if (_droppedDuringDrain > 0)
        {
            Logger.Warning("Dropped " + _droppedDuringDrain + " surplus once-per-mission traps from the Item Queue");
        }

        lock (_itemQueueLock)
        {
            File.WriteAllLines(_itemQueueFile, _itemQueue);
        }
    }

    public void ProcessAPItem(string apItemName)
    {
        try
        {
            string itemName = _items[apItemName].GameId;

            Logger.Msg("Received Item: " + apItemName);

            
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
        catch (KeyNotFoundException)
        {
            Logger.Error("Unknown Item '" + apItemName + "'");
            return;
        }
        catch (Il2CppException)
        {
            Logger.Error("Unknown Item '" + apItemName + "'");
            return;
        }
        catch (NullReferenceException)
        {
            if (!TryEnqueue(apItemName))
            {
                if (_draining)
                {
                    _droppedDuringDrain++;
                }
                else
                {
                    Logger.Warning("Dropped '" + apItemName + "', already queued " + ONCE_PER_MISSION_TRAP_QUEUE_LIMIT + " times");
                }
                return;
            }

            if (_draining)
            {
                return;
            }

            lock (_itemQueueLock)
            {
                File.WriteAllLines(_itemQueueFile, _itemQueue);
            }
        }
    }

    private void HandlePunchcardItem(string punchcardName)
    {
        Logger.Msg("Processing Punchcard: " + punchcardName);

        // guard against duplicate punchcards
        if (ProgressionManager.Instance.IsCardUnlocked(punchcardName))
        {
            Logger.Msg("Punchcard already unlocked");
            return;
        }

        PunchcardDefinitionV2 punchcard = GraphUtil.GetPunchcardDefinitionById(punchcardName);
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

        Logger.Msg("Added Punchcard: " + punchcardName);

        RefreshMapCards();
    }

    // refresh mission map, so unlocks may be visible on mission finish
    private static void RefreshMapCards()
    {
        MapCardManager mapCardManager = Object.FindObjectOfType<MapCardManager>();
        if (mapCardManager == null)
        {
            return;
        }

        mapCardManager.UpdateMapCards();
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
            case "SpawnRequisitionPoints":
            {
                Random rand = new();
                MissionStatsTracker.Instance.AddRequisitionPoints(rand.Next(10, 151));
                break;
            }
            case "SpawnSpotter":
            {
                PunchcardDefinitionV2 spotterCard = GraphUtil.GetPunchcardDefinitionById("Spotter");

                string spotterId = null;
                EntityRoles spotterRole = EntityRoles.Ally;
                TextIdentifier spotterDisplayName = null;
                int spotterHealth = 0;
                int spotterArmour = 0;
                string spotterIconID = null;
                MapEntityStates spotterState = MapEntityStates.None;
                
                foreach (Node node in spotterCard.Graph.nodes)
                {
                    State_SpawnMapEntity mapEntityCandidate = node.TryCast<State_SpawnMapEntity>();
                    if (mapEntityCandidate != null)
                    {
                        bool isSpotter = mapEntityCandidate.Role == EntityRoles.Spotter;
                        spotterId = mapEntityCandidate.ID;
                        spotterRole = mapEntityCandidate.Role;
                        spotterDisplayName = mapEntityCandidate.DisplayName;
                        spotterHealth = mapEntityCandidate.Health;
                        spotterArmour = mapEntityCandidate.Armour;
                        spotterIconID = mapEntityCandidate.Icon.ID;
                        spotterState = mapEntityCandidate.StartingState;
                        break;
                    }
                }

                if (spotterId == null)
                {
                    Logger.Warning("Could not find Spotter Node");
                    break;
                }

                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = MissionUtil.RandomZone(spotterRole);
                GridReference gridRef = spawnZone.GetRandomGridPosition(spawnRand);
                Vector3 pos = gridRef.GetLocation(FireMission.Instance.GetGridBounds());

                MapEntity spotterEntity = FireMission.Instance.CreateMapEntity(spotterId, spotterDisplayName, 0, pos, spotterRole, spotterHealth, spotterArmour, 0, spotterState, spotterIconID);
                FireMission.Instance.RegisterMapEntity(spotterEntity);

                GraphUtil.SubmitTeleprinterReport(spotterCard, spotterEntity.ID);

                break;
            }
            case "SpawnLocationReport":
            {
                PunchcardDefinitionV2 convoyCard = GraphUtil.GetPunchcardDefinitionById("LocationReport");

                string convoyId = null;
                EntityRoles convoyRole = EntityRoles.Ally;
                TextIdentifier convoyDisplayName = null;
                int convoyHealth = 0;
                int convoyArmour = 0;
                string convoyIconID = null;
                MapEntityStates convoyState = MapEntityStates.None;
                
                foreach (Node node in convoyCard.Graph.nodes)
                {
                    State_SpawnMapEntity mapEntityCandidate = node.TryCast<State_SpawnMapEntity>();
                    if (mapEntityCandidate != null)
                    {
                        bool isConvoy = mapEntityCandidate.Role == EntityRoles.Spotter;
                        convoyId = mapEntityCandidate.ID;
                        convoyRole = mapEntityCandidate.Role;
                        convoyDisplayName = mapEntityCandidate.DisplayName;
                        convoyHealth = mapEntityCandidate.Health;
                        convoyArmour = mapEntityCandidate.Armour;
                        convoyIconID = mapEntityCandidate.Icon.ID;
                        convoyState = mapEntityCandidate.StartingState;
                        break;
                    }
                }

                if (convoyId == null)
                {
                    Logger.Warning("Could not find LocationReport Node");
                    break;
                }

                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = MissionUtil.RandomZone(convoyRole);
                GridReference gridRef = spawnZone.GetRandomGridPosition(spawnRand);
                Vector3 pos = gridRef.GetLocation(FireMission.Instance.GetGridBounds());

                MapEntity convoyEntity = FireMission.Instance.CreateMapEntity(convoyId, convoyDisplayName, 0, pos, convoyRole, convoyHealth, convoyArmour, 0, convoyState, convoyIconID);
                FireMission.Instance.RegisterMapEntity(convoyEntity);

                GraphUtil.SubmitTeleprinterReport(convoyCard, convoyEntity.ID);

                break;
            }
        }
    }

    private void HandleTrapItem(string trapName)
    {
        // traps touch a lot of live scene state (engine, valves, shell pool, map entities, etc.),
        // so they must only fire in a fully initialized, still running mission.
        if (!MissionReadyGuard.IsReady())
        {
            throw new NullReferenceException("Mission not ready for traps");
        }

        switch (trapName)
        {
            case "TrapFillMagazine":
            {
                PunchcardDefinitionV2 starShellCard = GraphUtil.GetPunchcardDefinitionById("STARShell");
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
                    Logger.Warning("Shell configured for Trap not found (STARShell)");
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
                    Logger.Msg("Trap Magazine Fill completed");
                }

                break;
            }
            case "TrapEmergencyMove":
            {
                PunchcardDefinitionV2 emergencyMoveCard = GraphUtil.GetPunchcardDefinitionById("MoveZone");

                LocationSelection moveTarget = null;
                foreach (Node node in emergencyMoveCard.Graph.nodes)
                {
                    State_MoveTurret moveCandidate = node.TryCast<State_MoveTurret>();
                    if (moveCandidate != null)
                    {
                        moveTarget = moveCandidate.LocationToMoveTo;
                        break;
                    }
                }

                if (moveTarget == null)
                {
                    Logger.Warning("Could not find Move Turret Node (MoveZone)");
                    break;
                }

                if (moveTarget.LocationType == LocationSelection.LocationTypes.Relative
                    && moveTarget.RelativeTo == LocationSelection.RelativeReferenceTypes.Self
                    && moveTarget.RelativeDirection == LocationSelection.RelativeDirections.RandomInRadius)
                {
                    float minDistance = moveTarget.DistanceMin.Get(null);
                    float maxDistance = moveTarget.DistanceMax.Get(null);

                    Vector3 currentPos = TurretController.Instance.transform.position;
                    float angleRad = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    float distance = UnityEngine.Random.Range(minDistance, maxDistance);
                    Vector3 offset = new Vector3(Mathf.Cos(angleRad) * distance, 0f, Mathf.Sin(angleRad) * distance);
                    Vector3 targetPos = GridReference.ClampToGridBounds(currentPos + offset, FireMission.Instance.GetGridBounds());

                    TurretController.Instance.MoveTurret(targetPos);

                    GraphUtil.SubmitTeleprinterReport(emergencyMoveCard, "");

                    break;
                }

                if (moveTarget.LocationType == LocationSelection.LocationTypes.Relative)
                {
                    Logger.Warning(
                        "Emergency Move card uses unsupported Relative config: RelativeTo="
                        + moveTarget.RelativeTo + " RelativeDirection=" + moveTarget.RelativeDirection);
                    break;
                }

                if (moveTarget.LocationType != LocationSelection.LocationTypes.Zone)
                {
                    Logger.Warning("Emergency Move card uses unsupported LocationType: " + moveTarget.LocationType);
                    break;
                }

                Zone targetZone = null;
                foreach (Zone zone in MissionManager.Instance.CurrentMission.Zones)
                {
                    if (zone.ID == moveTarget.ZoneID)
                    {
                        targetZone = zone;
                        break;
                    }
                }

                if (targetZone == null)
                {
                    Logger.Warning("Could not find Zone '" + moveTarget.ZoneID + "' for Emergency Move");
                    break;
                }

                Il2CppSystem.Random moveRand = new();
                GridReference moveGridRef = targetZone.GetRandomGridPosition(moveRand);
                Vector3 movePos = moveGridRef.GetLocation(FireMission.Instance.GetGridBounds());

                TurretController.Instance.MoveTurret(movePos);

                GraphUtil.SubmitTeleprinterReport(emergencyMoveCard, "");

                break;
            }
            case "TrapCounterBattery":
            {
                // bail out on those two missions, since they are not built for CB and do not have the ordnance for it.
                if (TutorialGuard.IsTutorial())
                {
                    throw new NullReferenceException("Mission 1 or 2 are not completable with CBT");
                }
                
                // Not every mission has a CounterBatteryTimer instance, it's only instantiated by
                // the mission's own State_StartTimer node once that node's trigger fires.
                // Interestingly enough, mission 1 and 3 don't have the node at all, while mission 2 does for some reason
                // (maybe mission 2 was originally planned as the CB introduction? Who knows).
                // This method spawns it ourselves if the node exists but hasn't fired yet.
                // if the game's own trigger fires later in the same mission,
                // CBTimerDuplicationHandler (Harmony patch on State_StartTimer.OnEnter) destroys whichever instance
                // goes stale so only one timer survives.
                CounterBatteryTimer cbtInstance = CBHelper.GetOrSpawnCbtTimer();
                cbtInstance.AddTime(CBT_DEFAULT_TIME);
                
                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = MissionUtil.RandomZone(EntityRoles.Enemy);
                GridReference gridRef = spawnZone.GetRandomGridPosition(spawnRand);
                Vector3 pos = gridRef.GetLocation(FireMission.Instance.GetGridBounds());

                MapEntity artyEntity = FireMission.Instance.CreateMapEntity(
                    CB_DEFAULT_ID,
                    new TextIdentifier("APArtillery"),
                    0,
                    pos,
                    EntityRoles.Artillery | EntityRoles.Enemy,
                    1,
                    0,
                    1,
                    MapEntityStates.None,
                    "Enemy Field Artillery"
                );
                FireMission.Instance.RegisterMapEntity(artyEntity);

                List<string> cbtLinesPrimary = new List<string>();
                List<string> cbtLinesSecondary = new List<string>();
                
                cbtLinesPrimary.Add("COUNTER BATTERY");
                cbtLinesPrimary.Add("COUNTER BATTERY");
                cbtLinesPrimary.Add("COUNTER BATTERY");
                
                cbtLinesSecondary.Add("------------------------------------");
                cbtLinesSecondary.Add("ENEMY ARTILLERY SPOTTED IN SECTOR " + gridRef.Location);
                cbtLinesSecondary.Add("------------------------------------");

                TeleprinterHelper.printTextOnPrimary(cbtLinesPrimary, Teleprinter.TeleprinterAlarmState.High);
                TeleprinterHelper.printTextOnSecondary(cbtLinesPrimary, Teleprinter.TeleprinterAlarmState.High);
                
                cbtInstance.StartTimer();

                break;
            }
            case "TrapSabotage":
            {
                // bail out on those two missions, since the right gun valves cannot be repaired.
                if (TutorialGuard.IsTutorial())
                {
                    throw new NullReferenceException("valves on right gun cannot be repaired in Mission 1 or 2");
                }
                
                DieselEngineController engine = Object.FindFirstObjectByType<DieselEngineController>();

                if (!engine.EnginesRunning)
                {
                    throw new NullReferenceException("Engine is off already");
                }

                if (_sabotageThisMission)
                {
                    throw new NullReferenceException("Sabotaged already");
                }
                
                DieselEngineStateRelay relay = Object.FindFirstObjectByType<DieselEngineStateRelay>();
                if (relay == null)
                {
                    throw new NullReferenceException("DieselEngineStateRelay not found");
                }

                // pick the valves before changing any state, so a failure doesn't leave a half-applied sabotage
                Random rand = new();
                int numOfSabotagedValves = rand.Next(VALVE_SABOTAGE_MIN, VALVE_SABOTAGE_MAX + 1);// + 1 because it's excluding
                System.Collections.Generic.List<ValveController> valves = new();
                for (int i = 0; i < numOfSabotagedValves; i++)
                {
                    ValveController valve = HighPressureSystemManager.GetRandomRegisteredValveAcrossAllSystems();
                    if (valve == null)
                    {
                        throw new NullReferenceException("No valid registered valve found");
                    }
                    valves.Add(valve);
                }

                _sabotageThisMission = true;
                relay.ForceEngineOff();
                foreach (ValveController valve in valves)
                {
                    valve.DamageValve();
                }
                
                List<string> sabotageLinesPrimary = new();
                List<string> sabotageLinesSecondary = new();
                
                sabotageLinesPrimary.Add("SABOTAGE");
                sabotageLinesPrimary.Add("SABOTAGE");
                sabotageLinesPrimary.Add("SABOTAGE");
                sabotageLinesSecondary.Add("----------------------------------------------------");
                sabotageLinesSecondary.Add("ANTI-MONARCHISTS HAVE SABOTAGED YOUR IRON NEST");
                sabotageLinesSecondary.Add("ENGINES HAVE BEEN DISABLED");
                sabotageLinesSecondary.Add("PRESSURE DROPS IN SEVERAL CRITICAL SYSTEMS DETECTED");
                sabotageLinesSecondary.Add("----------------------------------------------------");
                
                TeleprinterHelper.printTextOnPrimary(sabotageLinesPrimary, Teleprinter.TeleprinterAlarmState.High);
                TeleprinterHelper.printTextOnSecondary(sabotageLinesSecondary, Teleprinter.TeleprinterAlarmState.High);
                
                break;
            }
        }
    }

    // drains the item queue once per mission, as soon as that mission is fully initialized
    public void ProcessPendingMissionLoad()
    {
        int generation = MissionReadyGuard.MissionGeneration;
        if (generation == _drainedMissionGeneration || !MissionReadyGuard.IsReady())
        {
            return;
        }

        _drainedMissionGeneration = generation;
        _sabotageThisMission = false;
        DrainQueue();
    }
}
