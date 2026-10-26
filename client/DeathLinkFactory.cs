using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Converters;

namespace APNestClient;

public class DeathLinkFactory
{
    private readonly Random _random = new();
    
    private string _slotName;
    private Dictionary<double, List<string>> _handledDeathLinks = new();
    
    private List<string> _deathMessages = new()
    {
        "High Command Requests 1 second of silence for the death of {{player}}.",
        "{{player}} failed Castille. His Family will be punished accordingly.",
        "{{player}} misread their orders. Forward Observer #2 paid the price for it.",
        "{{player}} was added to the daily casualty report.",
        "{{player}} bravely defended the Crown. Bravely, but the Enemy won. Disgraceful.",
        "Today's Lottery for posthumous promotions drew {{player}}'s name.",
    };

    public DeathLinkFactory(
        string slotName
    )
    {
        _slotName = slotName;
    }
    
    public DeathLink CreateDeathLink()
    {
        string cause = _deathMessages[_random.Next(_deathMessages.Count)];
        cause = cause.Replace("{{player}}", _slotName);
        
        return new DeathLink(_slotName, cause);
    }

    public bool WasHandled(DeathLink deathLink)
    {
        double ts = deathLink.Timestamp.ToUnixTimeStamp();
        string source = deathLink.Source;

        if (_handledDeathLinks.ContainsKey(ts))
        {
            List<string> dlList =  _handledDeathLinks[ts];
            if (dlList.Contains(source))
            {
                return true;
            }
            
            dlList.Add(source);
            _handledDeathLinks[ts] = dlList;
            return false;
        }
        
        
        _handledDeathLinks.Add(ts, [source]);
        return false;
    }

    public void AddOwnDLToHandledList(DeathLink deathLink)
    {
        double ts = deathLink.Timestamp.ToUnixTimeStamp();
        string source = deathLink.Source;

        if (_handledDeathLinks.ContainsKey(ts))
        {
            List<string> dlList = _handledDeathLinks[ts];
            dlList.Add(source);
            return;
        }
        
        _handledDeathLinks.Add(ts, [source]);
    }
}