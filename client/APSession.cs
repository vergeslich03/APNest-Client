using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
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
    private ConcurrentQueue<string> _queue = new();

    private APConnectionState _connectionState = APConnectionState.Disconnected;
    private string _loginFailureReason;

    private ArchipelagoSession _session;
    
    public static event Action<string> ItemReceived;

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

            List<string> locationBatch = new List<string>();
            while (_queue.TryDequeue(out string location))
            {
                locationBatch.Add(location);
            }
            
            SendLocationChecks(locationBatch.ToArray());
            
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
                            _queue.Enqueue(locationName);
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
            _queue.Enqueue(locationName);
        }   
    }

    private long[] APNamesToIDs(string[] names)
    {
        return names.Select(name =>
            _session.Locations.GetLocationIdFromName("IRON NEST: Heavy Turret Simulator", name)
        ).ToArray();
    }

    public void ReceiveItem()
    {
        string itemName = GetApItemName(_session.Items.DequeueItem().ItemId);
        ItemReceived?.Invoke(itemName);
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