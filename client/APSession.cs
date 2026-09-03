using System;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using APNestClient.ModLoader;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;

namespace APNestClient;

public enum APConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Failed,
}

public class APSession
{
    public static readonly string DataDirectory = ModLoaderPaths.DataDirectory;
    private readonly string _locationsQueueFile;
    private readonly string _itemIndexFile;
    private readonly string _seedFile;

    private static object _locationQueueLock = new object();
    private static object _itemIndexLock = new object();


    private ConcurrentQueue<string> _queue = new();
    private ConcurrentQueue<string> _pendingItemNames = new();

    private APConnectionState _connectionState = APConnectionState.Disconnected;
    private string _loginFailureReason;

    private ArchipelagoSession _session;
    private Dictionary<string, object> _slotData;

    private int _lastItemIndexHandled = -1;
    private string _goal;
    private string _medalTier;
    private List<string> _neededChecksForGoal = new();
    private bool _sentGoal = false;
    
    public static event Action<string> ItemReceived;

    public APSession()
    {
        _locationsQueueFile = Path.Combine(DataDirectory, "LocationsQueue.txt");
        _itemIndexFile = Path.Combine(DataDirectory, "ItemIndex.txt");
        _seedFile = Path.Combine(DataDirectory, "Seed.txt");

        if (!Directory.Exists(DataDirectory))
        {
            Directory.CreateDirectory(DataDirectory);
        }

        if (!File.Exists(_locationsQueueFile))
        {
            File.Create(_locationsQueueFile).Close();
        }

        string[] persistedQueue = File.ReadAllLines(_locationsQueueFile);
        foreach (string persistedQueueItem in persistedQueue)
        {
            _queue.Enqueue(persistedQueueItem);
        }
    }

    public void Connect(string host, int port, string slotName, string password)
    {
        if (_connectionState == APConnectionState.Connected)
        {
            return;
        }
        
        _connectionState = APConnectionState.Connecting;
        try
        {
            _session = ArchipelagoSessionFactory.CreateSession(host, port);
            LoginResult connResult = _session.TryConnectAndLogin(
                "IRON NEST: Heavy Turret Simulator",
                slotName,
                ItemsHandlingFlags.IncludeOwnItems,
                new Version(0, 6, 7),
                ["AP"],
                null,
                password
            );

            if (connResult is LoginFailure errorResult)
            {
                _connectionState = APConnectionState.Failed;
                _loginFailureReason = errorResult.Errors[0];
                Logger.Warning("Failed to connect to Archipelago: " + _loginFailureReason);
                return;
            }

            var login = (LoginSuccessful)connResult;
            _slotData = login.SlotData;
            
            _goal = _slotData.TryGetValue("goal",  out var goal) ? goal.ToString() : "mission_15";
            _medalTier = _slotData.TryGetValue("medal_tier", out var medalTier) ? medalTier.ToString() : "bronze";
            
            _connectionState = APConnectionState.Connected;
            Logger.Msg("Successfully Connected to Archipelago, have fun!");

            BuildGoalChecklist();
            
            string currentSeed = _session.RoomState.Seed;
            if (!File.Exists(_seedFile))
            {
                File.Create(_seedFile).Close();
                List<string> tmpList = new();
                tmpList.Add(currentSeed);
                File.WriteAllLines(_seedFile, tmpList);
            }
            if (File.ReadAllLines(_seedFile)[0] != currentSeed)
            {
                Logger.Warning("New Seed detected");

                lock (_locationQueueLock)
                {
                    _queue.Clear();
                    File.WriteAllLines(_locationsQueueFile, _queue);
                }
                
                _pendingItemNames.Clear();
                _sentGoal = false;

                List<string> tmpList = new();
                tmpList.Add(currentSeed);
                File.WriteAllLines(_seedFile, tmpList);
                File.Delete(_itemIndexFile);
                ProgressionManager.Instance.ResetAllUserProgress();
            }
            
            List<string> locationBatch = new List<string>();
            while (_queue.TryDequeue(out string location))
            {
                locationBatch.Add(location);
            }
            
            SendLocationChecks(locationBatch.ToArray());

            lock (_locationQueueLock)
            {
                File.WriteAllLines(_locationsQueueFile, _queue);
            }
            
            Logger.Msg("Location Check Queue flushed.");

            try
            {
                Int32.TryParse(File.ReadAllLines(_itemIndexFile)[0], out _lastItemIndexHandled);
            }
            catch (FileNotFoundException)
            {
            }
            

            int itemCounter = 0;
            while (_session.Items.Any())
            {
                if (itemCounter <= _lastItemIndexHandled)
                {
                    _session.Items.DequeueItem();
                    itemCounter++;
                    continue;
                }
                
                ReceiveItem();
                itemCounter++;
            }
            
            Logger.Msg("Pending Items flushed.");

            _session.Items.ItemReceived += x => ReceiveItem();
        }
        catch (Exception e)
        {
            _connectionState = APConnectionState.Failed;
            _loginFailureReason = e.Message;
            Logger.Warning("Failed to open Archipelago Connection: " + e.Message);
        }
    }
    
    public void Disconnect()
    {
        _session.Socket.DisconnectAsync().GetAwaiter().GetResult();
        _connectionState = APConnectionState.Disconnected;
    }

    public void SendLocationChecks(string[] locationNames)
    {
        Logger.Msg("Sending location checks to Archipelago");
        if (_connectionState == APConnectionState.Connected)
        {
            long[] locationIDs = APNamesToIDs(locationNames);
            
            _session.Locations.CompleteLocationChecksAsync(locationIDs).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        foreach (var locationName in locationNames)
                        {
                            EnqueueLocation(locationName);
                        }
                        
                        Logger.Warning("Could not Send location checks.");
                        Logger.Warning("Reason: " + (t.Exception?.InnerExceptions[0].Message ?? "canceled"));
                    }
                    
                    CheckGoalCompletion();
                }
            );
            
            return;
        }
        
        Logger.Warning("Client Not Connected to Archipelago");
        foreach (var locationName in locationNames)
        {
            EnqueueLocation(locationName);
        }   
    }

    private void EnqueueLocation(string locationName)
    {
        lock (_locationQueueLock)
        {
            _queue.Enqueue(locationName);
            File.WriteAllLines(_locationsQueueFile, _queue);
        }
    }

    private long[] APNamesToIDs(string[] names)
    {
        List<long> foundIds = new();
        foreach (string name in names)
        {
            long id = _session.Locations.GetLocationIdFromName("IRON NEST: Heavy Turret Simulator", name);
            if (id == -1)
            {
                Logger.Warning("Could not find location ID for: " + name);
                continue;
            }
            
            foundIds.Add(id);
        }
        
        return foundIds.ToArray();
    }

    public void ReceiveItem()
    {
        string itemName = GetApItemName(_session.Items.DequeueItem().ItemId);
        IncreaseHandledItemIndex();
        _pendingItemNames.Enqueue(itemName);
    }

    public void ProcessPendingItems()
    {
        while (_pendingItemNames.TryDequeue(out string itemName))
        {
            ItemReceived?.Invoke(itemName);
        }
    }

    private void BuildGoalChecklist()
    {
        _neededChecksForGoal.Clear();
        
        switch (_goal)
        {
            case "mission_15":
            {
                _neededChecksForGoal.Add("Mission 15: White Shells");
                break;
            }
            case "all_endings":
            {
                _neededChecksForGoal.Add("Mission 15: White Shells - E1 Bronze");
                _neededChecksForGoal.Add("Mission 15: White Shells - E2 Bronze");
                _neededChecksForGoal.Add("Mission 15: White Shells - E3 Bronze");
                _neededChecksForGoal.Add("Mission 15: White Shells - E4 Bronze");
                break;
            }
            case "all_medals":
            {
                LookupTables lookuptable = new LookupTables(LookupTables.TableType.MedalLocations);
                foreach ((string medal, string check) in lookuptable.MedalNameToAPLocationNameTable)
                {
                    string medalLower = check.ToLower();
                    if (medalLower.EndsWith(_medalTier) && !medalLower.StartsWith("mission 15"))
                    {
                        _neededChecksForGoal.Add(check);
                    }
                }
                break;
            }
        }
    }

    private void CheckGoalCompletion()
    {
        if (_sentGoal || _connectionState != APConnectionState.Connected)
        {
            return;
        }

        HashSet<long> sentChecks = new HashSet<long>(_session.Locations.AllLocationsChecked);
        long[] required = APNamesToIDs(_neededChecksForGoal.ToArray());

        if (required.Length == 0 || required.Length != _neededChecksForGoal.Count)
        {
            return;
        }

        if (!required.All(sentChecks.Contains))
        {
            return;
        }
        _session.SetGoalAchieved();
        _sentGoal = true;
        Logger.Msg("Goal achieved, congratulations!");
    }

    private void IncreaseHandledItemIndex()
    {
        lock (_itemIndexLock)
        {
            _lastItemIndexHandled++;
            List<string> tmpList = new List<string>();
            tmpList.Add(_lastItemIndexHandled.ToString());
            File.WriteAllLines(_itemIndexFile, tmpList);
        }
    }

    public string GetApItemName(long itemId)
    {
        return _session.Items.GetItemName(itemId);
    }

    public APConnectionState GetConnectionState()
    {
        return _connectionState;
    }

    public string GetLoginFailureReason()
    {
        return _loginFailureReason;
    }
}