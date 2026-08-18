using System;
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
    private string _host;
    private int _port;
    private string _slotName;
    private string _password;

    private APConnectionState _connectionState = APConnectionState.Disconnected;
    private string _loginFailureReason;
    
    public APSession(string host, int port, string slotName, string password)
    {
        _host = host;
        _port = port;
        _slotName = slotName;
        _password = password;
    }

    public void Connect()
    {
        _connectionState = APConnectionState.Connecting;
        try
        {
            ArchipelagoSession session = ArchipelagoSessionFactory.CreateSession(_host, _port);
            LoginResult connResult = session.TryConnectAndLogin(
                "IRON NEST: Heavy Turret Simulator",
                _slotName,
                ItemsHandlingFlags.IncludeOwnItems,
                new Version(0, 6, 7),
                ["AP"],
                null,
                _password
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
    }
    
    public void SendLocationCheck()
    {
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