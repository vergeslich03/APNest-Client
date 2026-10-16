using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;

namespace APNestClient;

public class DeathLinkHandler
{
    private bool _enableDeathLink = false;
    
    private ConnectUI _connectUi;
    private DeathLinkService _deathLinkService { get; }
    
    private List<string> _deathMessages = new List<string>()
    {
        "{{player}} put up a valiant fight.",
        "{{player}} made their last stand.",
        "{{player}} won't be reporting for duty tomorrow.",
        "{{player}} died a hero's death defending Castile.",
        "{{player}} fell victim to Anti-Monarchist Counter-Battery.",
        "{{player}} misread the orders from High Command.",
        "{{player}} trusted an Anti-Monarchist observer's coordinates.",
        "{{player}} was found guilty of dying without orders.",
        "{{player}} has been posthumously promoted.",
        "{{player}} heard the counter-battery a second too late.",
        "{{player}}'s family was billed for the wasted ammunition.",
        "{{player}} confused 'elevate' with 'evacuate'. Neither happened in time.",
        "{{player}} thought the Anti-Monarchists would miss. They did not.",
        "Congratulations to {{player}}, this week's winner of the Posthumous Medal of Valor raffle.",
        "Rest in pieces, {{player}}. Several pieces, actually.",
        "Witnesses describe {{player}}'s final words as 'is this thing loaded?'",
        "Due to budget cuts, {{player}}'s funeral will be held inside the shell casing.",
        "Against all odds and several warning signs, {{player}} has died.",
        "{{player}} died believing in the King. The King never learned their name.",
        "By order of His Majesty, {{player}}'s death is not to be discussed.",
        "{{player}} failed the King. The King will remember.",
        "{{player}} failed their mission. The firing squad has been notified.",
        "{{player}}'s target remains undamaged. The surrounding countryside, less so.",
        "Five thousand tons of steel, and somehow {{player}} still missed.",
    };

    DeathLinkHandler(
        ConnectUI connectUi,
        ArchipelagoSession apSession
    )
    {
        _connectUi = connectUi;
        _deathLinkService = apSession.CreateDeathLinkService();
    }
    
    public DeathLink CreateDeathLink()
    {
        Random random = new();
        string player = _connectUi.GetApSlotName();
        
        string cause = _deathMessages[random.Next(_deathMessages.Count)];
        cause = cause.Replace("{{player}}", player);
        
        return new DeathLink(player,cause);
    }

    public void SendDeathLink(DeathLink deathLink)
    {
        if (!_enableDeathLink) return;
        
        _deathLinkService.SendDeathLink(deathLink);
    }

    public void SetDeathLinkStatus(bool status)
    {
        _enableDeathLink = status;
    }
}