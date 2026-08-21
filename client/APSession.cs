using System;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using MelonLoader;
using MelonLoader.Utils;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Il2Cpp;

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
    private readonly string _dataDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "APNestClient");
    private readonly string _locationsQueueFile;
    private static object _locationQueueLock = new object();

    private readonly string _seedFile;
    
    private ConcurrentQueue<string> _queue = new();
    private ConcurrentQueue<string> _pendingItemNames = new();

    private APConnectionState _connectionState = APConnectionState.Disconnected;
    private string _loginFailureReason;

    private ArchipelagoSession _session;
    
    public static event Action<string> ItemReceived;

    public APSession()
    {
        _locationsQueueFile = Path.Combine(_dataDirectory, "LocationsQueue.txt");
        _seedFile = Path.Combine(_dataDirectory, "Seed.txt");

        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
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
                MelonLogger.Warning("Failed to connect to Archipelago: " + _loginFailureReason);
                return;
            }
            
            _connectionState = APConnectionState.Connected;
            MelonLogger.Msg("Successfully Connected to Archipelago, have fun!");

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
                MelonLogger.Warning("New Seed detected");

                lock (_locationQueueLock)
                {
                    _queue.Clear();
                    File.WriteAllLines(_locationsQueueFile, _queue);
                }
                
                _pendingItemNames.Clear();

                List<string> tmpList = new();
                tmpList.Add(currentSeed);
                File.WriteAllLines(_seedFile, tmpList);
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
            
            MelonLogger.Msg("Location Check Queue flushed.");
            

            while (_session.Items.Any())
            {
                ReceiveItem();
            }
            
            MelonLogger.Msg("Pending Items flushed.");

            _session.Items.ItemReceived += x => ReceiveItem();
        }
        catch (Exception e)
        {
            _connectionState = APConnectionState.Failed;
            _loginFailureReason = e.Message;
            MelonLogger.Warning("Failed to open Archipelago Connection: " + e.Message);
        }
    }
    
    public void Disconnect()
    {
        _session.Socket.DisconnectAsync().GetAwaiter().GetResult();
        _connectionState = APConnectionState.Disconnected;
    }

    public void SendLocationChecks(string[] locationNames)
    {
        MelonLogger.Msg("Sending location checks to Archipelago");
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
                        
                        MelonLogger.Warning("Could not Send location checks.");
                        MelonLogger.Warning("Reason: " + (t.Exception?.InnerExceptions[0].Message ?? "canceled"));
                    }
                }
            );
            return;
        }
        
        MelonLogger.Warning("Client Not Connected to Archipelago");
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
                MelonLogger.Warning("Could not find location ID for: " + name);
                continue;
            }
            
            foundIds.Add(id);
        }
        
        return foundIds.ToArray();
    }

    public void ReceiveItem()
    {
        string itemName = GetApItemName(_session.Items.DequeueItem().ItemId);
        _pendingItemNames.Enqueue(itemName);
    }

    public void ProcessPendingItems()
    {
        while (_pendingItemNames.TryDequeue(out string itemName))
        {
            ItemReceived?.Invoke(itemName);
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