using System;
using System.Collections.Concurrent;
using System.IO;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppLocalisation;
using Il2CppSleepyNodes;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Random = System.Random;

namespace APNestClient;

public class ItemReceiver
{
    private string _itemQueueFile;
    private object _itemQueueLock = new();
    
    private LookupTables _lookupTable;
    
    private ConcurrentQueue<string> _itemQueue = new();
    private bool _missionChangedSubscribed;
    private volatile bool _pendingMissionLoad;
    private volatile string _pendingMissionId;

    public ItemReceiver()
    {
        _itemQueueFile = Path.Combine(APSession.DataDirectory, "ItemQueue.txt");
        
        _lookupTable = new LookupTables(LookupTables.TableType.Items);
        APSession.ItemReceived += itemName => ProcessAPItem(itemName);

        if (!File.Exists(_itemQueueFile))
        {
            File.Create(_itemQueueFile).Close();
        }

        foreach (string queuedItem in File.ReadAllLines(_itemQueueFile))
        {
            _itemQueue.Enqueue(queuedItem);
        }
    }

    private void DrainQueue()
    {
        while (_itemQueue.TryDequeue(out string itemName))
        {
            ProcessAPItem(itemName);
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
            string itemName = _lookupTable.ApItemNameToGameIdTable[apItemName];

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
            lock (_itemQueueLock)
            {
                File.WriteAllLines(_itemQueueFile, _itemQueue);
            }
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
            case "SpawnRequisitionPoints":
            {
                Random rand = new();
                MissionStatsTracker.Instance.AddRequisitionPoints(rand.Next(10, 150));
                break;
            }
            case "SpawnSpotter":
            {
                PunchcardDefinitionV2 spotterCard =  RequisitionConsoleManager.Instance.AllDefinitions["Spotter"];

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
                    MelonLogger.Warning("Could not find Spotter Node");
                    break;
                }

                List<Zone> allyZones = new();
                foreach (Zone zone in MissionManager.Instance.CurrentMission.Zones)
                {
                    if (zone.Role == EntityRoles.Ally)
                    {
                        allyZones.Add(zone);
                    }
                }

                if (allyZones.Count == 0)
                {
                    MelonLogger.Warning("No Ally zone found, falling back to Zones[0]");
                    allyZones.Add(MissionManager.Instance.CurrentMission.Zones[0]);
                }

                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = allyZones[spawnRand.Next(allyZones.Count)];
                GridReference gridRef = spawnZone.GetRandomGridPosition(spawnRand);
                Vector3 pos = gridRef.GetLocation(FireMission.Instance.GetGridBounds());

                MapEntity spotterEntity = FireMission.Instance.CreateMapEntity(spotterId, spotterDisplayName, 0, pos, spotterRole, spotterHealth, spotterArmour, 0, spotterState, spotterIconID);
                FireMission.Instance.RegisterMapEntity(spotterEntity);

                foreach (Node node in spotterCard.Graph.nodes)
                {
                    State_TeleprinterText teleprinterNode = node.TryCast<State_TeleprinterText>();
                    if (teleprinterNode != null)
                    {
                        SubmitTeleprinterReport(teleprinterNode, spotterEntity.ID);
                    }
                }

                break;
            }
            case "SpawnLocationReport":
            {
                PunchcardDefinitionV2 convoyCard =  RequisitionConsoleManager.Instance.AllDefinitions["LocationReport"];

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
                    MelonLogger.Warning("Could not find LocationReport Node");
                    break;
                }

                List<Zone> allyZones = new();
                foreach (Zone zone in MissionManager.Instance.CurrentMission.Zones)
                {
                    if (zone.Role == EntityRoles.Ally)
                    {
                        allyZones.Add(zone);
                    }
                }

                if (allyZones.Count == 0)
                {
                    MelonLogger.Warning("No Ally zone found, falling back to Zones[0]");
                    allyZones.Add(MissionManager.Instance.CurrentMission.Zones[0]);
                }

                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = allyZones[spawnRand.Next(allyZones.Count)];
                GridReference gridRef = spawnZone.GetRandomGridPosition(spawnRand);
                Vector3 pos = gridRef.GetLocation(FireMission.Instance.GetGridBounds());

                MapEntity convoyEntity = FireMission.Instance.CreateMapEntity(convoyId, convoyDisplayName, 0, pos, convoyRole, convoyHealth, convoyArmour, 0, convoyState, convoyIconID);
                FireMission.Instance.RegisterMapEntity(convoyEntity);

                foreach (Node node in convoyCard.Graph.nodes)
                {
                    State_TeleprinterText teleprinterNode = node.TryCast<State_TeleprinterText>();
                    if (teleprinterNode != null)
                    {
                        SubmitTeleprinterReport(teleprinterNode, convoyEntity.ID);
                    }
                }

                break;
            }
        }
    }
    
    private static void SubmitTeleprinterReport(State_TeleprinterText node, string entityId)
    {
        string text = node.Text != null ? node.Text.Get() : null;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        foreach (State_TeleprinterText.StringReplacement replacement in node.EntityIDToReplace)
        {
            text = text.Replace(replacement.Text, entityId);
        }

        List<string> lines = FireMissionTokenProcessor.ProcessBlock(text, new Dictionary<string, GridReference>());
        Teleprinter printer = Teleprinter.GetTeleprinter(node.Printer);
        printer.SubmitLines(Guid.NewGuid().ToString(), lines.Cast<IEnumerable<string>>(), null, node.WaitUntilComplete);

        if (node.AlarmState != Teleprinter.TeleprinterAlarmState.None)
        {
            printer.SignalAlarm(node.AlarmState);
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
            case "TrapEmergencyMove":
            {
                PunchcardDefinitionV2 emergencyMoveCard = RequisitionConsoleManager.Instance.AllDefinitions["MoveZone"];

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
                    MelonLogger.Warning("Could not find Move Turret Node (MoveZone)");
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

                    foreach (Node node in emergencyMoveCard.Graph.nodes)
                    {
                        State_TeleprinterText teleprinterNode = node.TryCast<State_TeleprinterText>();
                        if (teleprinterNode != null)
                        {
                            SubmitTeleprinterReport(teleprinterNode, "");
                        }
                    }

                    break;
                }

                if (moveTarget.LocationType == LocationSelection.LocationTypes.Relative)
                {
                    MelonLogger.Warning(
                        "Emergency Move card uses unsupported Relative config: RelativeTo="
                        + moveTarget.RelativeTo + " RelativeDirection=" + moveTarget.RelativeDirection);
                    break;
                }

                if (moveTarget.LocationType != LocationSelection.LocationTypes.Zone)
                {
                    MelonLogger.Warning("Emergency Move card uses unsupported LocationType: " + moveTarget.LocationType);
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
                    MelonLogger.Warning("Could not find Zone '" + moveTarget.ZoneID + "' for Emergency Move");
                    break;
                }

                Il2CppSystem.Random moveRand = new();
                GridReference moveGridRef = targetZone.GetRandomGridPosition(moveRand);
                Vector3 movePos = moveGridRef.GetLocation(FireMission.Instance.GetGridBounds());

                TurretController.Instance.MoveTurret(movePos);

                foreach (Node node in emergencyMoveCard.Graph.nodes)
                {
                    State_TeleprinterText teleprinterNode = node.TryCast<State_TeleprinterText>();
                    if (teleprinterNode != null)
                    {
                        SubmitTeleprinterReport(teleprinterNode, "");
                    }
                }

                break;
            }
            case "TrapCounterBattery":
            {
                CounterBatteryTimer cbtInstance = CounterBatteryTimer.Instance;
                
                List<Zone> enemyZones = new();
                foreach (Zone zone in MissionManager.Instance.CurrentMission.Zones)
                {
                    if (zone.Role == EntityRoles.Enemy)
                    {
                        enemyZones.Add(zone);
                    }
                }

                if (enemyZones.Count == 0)
                {
                    MelonLogger.Warning("No Enemy zone found, falling back to Zones[0]");
                    enemyZones.Add(MissionManager.Instance.CurrentMission.Zones[0]);
                }
                
                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = enemyZones[spawnRand.Next(enemyZones.Count)];
                GridReference gridRef = spawnZone.GetRandomGridPosition(spawnRand);
                Vector3 pos = gridRef.GetLocation(FireMission.Instance.GetGridBounds());

                MapEntity artyEntity = FireMission.Instance.CreateMapEntity(
                    "APArtillery",
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

                List<string> cbtLines = new List<string>();
                cbtLines.Add("--- COUNTER BATTERY ---");
                cbtLines.Add("ENEMY ARTILLERY SPOTTED IN SECTOR " + gridRef.Location);

                Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary)
                    .SubmitLines(
                        Guid.NewGuid().ToString(),
                        cbtLines.Cast<IEnumerable<string>>(),
                        null,
                        false
                    );
                
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