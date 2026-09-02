using System;
using System.Collections.Concurrent;
using System.IO;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppLocalisation;
using Il2CppSleepyNodes;
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
    
    
    private string _itemQueueFile;
    private object _itemQueueLock = new();

    private LookupTables _lookupTable;

    private ConcurrentQueue<string> _itemQueue = new();
    private bool _missionChangedSubscribed;
    private volatile bool _pendingMissionLoad;
    private volatile string _pendingMissionId;

    private bool _sabotageThisMission = false;

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
        _sabotageThisMission =  false;
        
        int count = _itemQueue.Count;
        for (int i = 0; i < count && _itemQueue.TryDequeue(out string itemName); i++)
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
            _itemQueue.Enqueue(apItemName);
            lock (_itemQueueLock)
            {
                File.WriteAllLines(_itemQueueFile, _itemQueue);
            }
        }
    }

    // Some vanilla IDs carry stray leading/trailing whitespaces
    // (like "CYANShell ", "WPShell ") - Since I use the trimmed version, getting the in-game Ids throws an error on dict lookup
    private static PunchcardDefinitionV2 GetPunchcardDefinition(string id)
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

    private void HandlePunchcardItem(string punchcardName)
    {
        Logger.Msg("Processing Punchcard: " + punchcardName);

        // guard against duplicate punchcards
        if (ProgressionManager.Instance.IsCardUnlocked(punchcardName))
        {
            Logger.Msg("Punchcard already unlocked");
            return;
        }

        PunchcardDefinitionV2 punchcard = GetPunchcardDefinition(punchcardName);
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
                PunchcardDefinitionV2 spotterCard = GetPunchcardDefinition("Spotter");

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
                    Logger.Warning("No Ally zone found, falling back to Zones[0]");
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
                PunchcardDefinitionV2 convoyCard = GetPunchcardDefinition("LocationReport");

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
                    Logger.Warning("No Ally zone found, falling back to Zones[0]");
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
                PunchcardDefinitionV2 starShellCard = GetPunchcardDefinition("STARShell");
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
                PunchcardDefinitionV2 emergencyMoveCard = GetPunchcardDefinition("MoveZone");

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
                // bail out on those two missions, since they are not built for CB and do not have the ordnance for it.
                if (MissionManager.Instance.CurrentMission.MissionID == "Hospital False Flag" ||
                    MissionManager.Instance.CurrentMission.MissionID == "ceremony and HCHE")
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
                CounterBatteryTimer cbtInstance = GetOrSpawnCbtTimer();
                cbtInstance.AddTime(CBT_DEFAULT_TIME);

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
                    Logger.Warning("No Enemy zone found, falling back to Zones[0]");
                    enemyZones.Add(MissionManager.Instance.CurrentMission.Zones[0]);
                }
                
                Il2CppSystem.Random spawnRand = new();
                Zone spawnZone = enemyZones[spawnRand.Next(enemyZones.Count)];
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

                Teleprinter teleprinterPrimary = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Primary);
                Teleprinter teleprinterSecondary = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary);
                
                teleprinterPrimary.SignalAlarm(Teleprinter.TeleprinterAlarmState.High);
                teleprinterSecondary.SignalAlarm(Teleprinter.TeleprinterAlarmState.High);

                teleprinterPrimary
                    .SubmitLines(
                        Guid.NewGuid().ToString(),
                        cbtLinesPrimary.Cast<IEnumerable<string>>(),
                        null,
                        false
                    );
                teleprinterSecondary
                    .SubmitLines(
                        Guid.NewGuid().ToString(),
                        cbtLinesSecondary.Cast<IEnumerable<string>>(),
                        null,
                        false
                    );
                
                cbtInstance.StartTimer();

                break;
            }
            case "TrapSabotage":
            {
                // bail out on those two missions, since the right gun valves cannot be repaired.
                if (MissionManager.Instance.CurrentMission.MissionID == "Hospital False Flag" ||
                    MissionManager.Instance.CurrentMission.MissionID == "ceremony and HCHE")
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
                
                _sabotageThisMission = true;
                
                DieselEngineStateRelay relay = Object.FindFirstObjectByType<DieselEngineStateRelay>();
                relay.ForceEngineOff();

                Random rand = new();
                int numOfSabotagedValves = rand.Next(VALVE_SABOTAGE_MIN, VALVE_SABOTAGE_MAX + 1);// + 1 because it's excluding
                for (int i = 0; i < numOfSabotagedValves; i++)
                {
                    ValveController valve = HighPressureSystemManager.GetRandomRegisteredValveAcrossAllSystems();
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
                
                Teleprinter teleprinterPrimary = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Primary);
                Teleprinter teleprinterSecondary = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary);
                
                teleprinterPrimary.SignalAlarm(Teleprinter.TeleprinterAlarmState.High);
                teleprinterSecondary.SignalAlarm(Teleprinter.TeleprinterAlarmState.High);

                teleprinterPrimary
                    .SubmitLines(
                        Guid.NewGuid().ToString(),
                        sabotageLinesPrimary.Cast<IEnumerable<string>>(),
                        null,
                        false
                    );
                teleprinterSecondary
                    .SubmitLines(
                        Guid.NewGuid().ToString(),
                        sabotageLinesSecondary.Cast<IEnumerable<string>>(),
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

    // Returns the live CounterBatteryTimer for the current mission, spawning it if the mission has a
    // State_StartTimer node but its in-mission trigger hasn't fired yet. Returns null
    // if there's no such node at all this mission (e.g. missions 1/3).
    private CounterBatteryTimer GetOrSpawnCbtTimer()
    {
        if (CounterBatteryTimer.Instance != null)
        {
            return CounterBatteryTimer.Instance;
        }

        if (MissionManager.Instance == null || MissionManager.Instance.CurrentMission == null)
        {
            return null;
        }

        State_StartTimer startTimerNode = null;
        foreach (Node node in MissionManager.Instance.CurrentMission.nodes)
        {
            startTimerNode = node.TryCast<State_StartTimer>();
            if (startTimerNode != null)
            {
                break;
            }
        }

        // Mission 3 has no State_StartTimer node of its own, so I borrow one from another mission's graph
        if (startTimerNode == null && MissionManager.Instance.CurrentOperation != null)
        {
            foreach (MissionNode missionNode in MissionManager.Instance.CurrentOperation.Missions)
            {
                if (missionNode.Mission == null || missionNode.Mission == MissionManager.Instance.CurrentMission)
                {
                    continue;
                }

                foreach (Node node in missionNode.Mission.nodes)
                {
                    startTimerNode = node.TryCast<State_StartTimer>();
                    if (startTimerNode != null)
                    {
                        break;
                    }
                }

                if (startTimerNode != null)
                {
                    Logger.Msg("GetOrSpawnCbtTimer: borrowing State_StartTimer from mission '" + missionNode.Mission.MissionID + "'");
                    break;
                }
            }
        }

        if (startTimerNode == null || startTimerNode.Prefab_BatteryTimer == null)
        {
            return null;
        }

        CounterBatteryTimer spawned = UnityEngine.Object.Instantiate(startTimerNode.Prefab_BatteryTimer);
        spawned.Init(startTimerNode.InitalTime);

        // Missions without a native CB encounter (e.g. mission 3) still have a static,
        // closed "Timer Hatch" prop baked into the base scene decorating the empty mount.
        // My spawned prefab brings its own hatch to the same slot, so disable the static one to stop the two from
        // overlapping/clipping into each other.
        foreach (Transform t in UnityEngine.Object.FindObjectsOfType<Transform>(true))
        {
            if (t.gameObject.name == "Timer Hatch" && t.parent == null)
            {
                t.gameObject.SetActive(false);
                break;
            }
        }

        return spawned;
    }
}