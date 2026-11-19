using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine.InputSystem;

namespace APNestClient.Data;

public enum ItemType
{
    Punchcard,
    Spawn,
    Trap
};

public sealed record ItemInfo(string APName, string GameId, ItemType Type);

public sealed record MedalInfo(string DisplayName, string Abbreviation);

public sealed record MissionInfo(string GameId, string APName, string[] Prerequisites, MedalInfo[] Medals);

public static class DataHolders
{
    public static readonly MissionInfo[] Missions =
    {
        new("Hospital False Flag", "Mission 01: Calibration Fire", [], [new("Ordnance Efficiency Laurel", "OE")]),
        new("ceremony and HCHE", "Mission 02: Fire and Light", [], [new("Ordnance Efficiency Citation", "OE")]),
        new("Insurrections and Requisitions", "Mission 03: Liberation", ["APShell", "PowderCharges"],
        [
            new("Measured Fire Star", "MF"), new("Marksman's Cross", "MC"), new("Unbroken Volley Medal", "UV"),
            new("Austere Service Medal", "AS")
        ]),
        new("Artillery Introduction", "Mission 04: Counter-Battery", [],
        [
            new("Salvo Commendation", "SC"), new("Counter-Battery Commendation", "CB"),
            new("Austere Service Medal", "AS"), new("No Quarter Cross", "NQ")
        ]),
        new("IronRoad", "Mission 05: Iron Road", [],
        [
            new("Measured Fire Star", "MF"), new("Unrelenting Volley Medal", "UV"),
            new("Ordnance Efficiency Cross", "OE"), new("Unaided Fire Distinction", "UF")
        ]),
        new("SiegeOfCartagena", "Mission 06: Siege of Cartagena", ["SMOKEShell"],
        [
            new("Measured Fire Star", "MF"), new("No Quarter Cross", "NQ"), new("Unaided Fire Distinction", "UF"),
            new("Austere Service Medal", "AS")
        ]),
        new("The Gorge", "Mission 07: The Gorge", [],
        [
            new("Measured Fire Star", "MF"), new("Unrelenting Volley Medal", "UV"), new("No Quarter Cross", "NQ"),
            new("Ordnance Efficiency Cross", "OE")
        ]),
        new("RockofGibraltar", "Mission 08: Rock of Gibraltar", [],
        [
            new("Measured Fire Star", "MF"), new("No Quarter Cross", "NQ"), new("Unaided Fire Distinction", "UF"),
            new("Austere Service Medal", "AS")
        ]),
        new("DeadReckoning", "Mission 09: Dead Reckoning", ["TEARShell"],
        [
            new("Measured Fire Star", "MF"), new("Unbroken Volley Medal", "UV"), new("No Quarter Cross", "NQ"),
            new("Austere Service Medal", "AS")
        ]),
        new("FireOnCall", "Mission 10: Fire on Call", [],
        [
            new("Measured Fire Star", "MF"), new("No Quarter Cross", "NQ"), new("Ordnance Efficiency Cross", "OE"),
            new("Unaided Fire Distinction", "UF")
        ]),
        new("HighTide", "Mission 11: High Tide", [],
        [
            new("Unbroken Volley Medal", "UV"), new("Salvo Commendation", "SC"), new("Marksman's Cross", "MC")
        ]),
        new("BlindFire", "Mission 12: Blind Fire", [],
        [
            new("Austere Service Medal", "AS"), new("Unbroken Volley Medal", "UV"), new("No Quarter Cross", "NQ"),
            new("Unaided Fire Distinction", "UF")
        ]),
        new("PhantomBattery", "Mission 13: Phantom Battery", ["MoveZone", "ScoutPlane"],
        [
            new("Unaided Fire Distinction", "UF"), new("No Quarter Cross", "NQ"), new("Marksman's Cross", "MC"),
            new("Counter-Battery Commendation", "CB")
        ]),
        new("FinalHarvest", "Mission 14: Final Harvest", [],
        [
            new("Measured Fire Star", "MF"), new("Unaided Fire Distinction", "UF"), new("No Quarter Cross", "NQ"),
            new("Rapid Engagement Medal", "RE")
        ]),
        new("WhiteShells", "Mission 15: White Shells", ["ATMCShell"],
        [
            new("Ending 1", "E1"), new("Ending 2", "E2"), new("Ending 3", "E3"), new("Ending 4", "E4")
        ]),
    };

    public static readonly ItemInfo[] Items =
    {
        new("Punchcard - HE Shell", "HEShell", ItemType.Punchcard),
        new("Punchcard - STAR Shell", "STARShell", ItemType.Punchcard),
        new("Punchcard - AP Shell", "APShell", ItemType.Punchcard),
        new("Punchcard - SMK Shell", "SMOKEShell", ItemType.Punchcard),
        new("Punchcard - TEAR Shell", "TEARShell", ItemType.Punchcard),
        new("Punchcard - INCN Shell", "INCNShell", ItemType.Punchcard),
        new("Punchcard - HCHE Shell", "HCHEShell", ItemType.Punchcard),
        new("Punchcard - DRIL Shell", "DRILShell", ItemType.Punchcard),
        new("Punchcard - LE Shell", "LEShell", ItemType.Punchcard),
        new("Punchcard - PHGN Shell", "PHGNShell", ItemType.Punchcard),
        new("Punchcard - WP Shell", "WPShell", ItemType.Punchcard),
        new("Punchcard - PCLM Shell", "PCLMShell", ItemType.Punchcard),
        new("Punchcard - APHE Shell", "APHEShell", ItemType.Punchcard),
        new("Punchcard - FLCH Shell", "FLCHShell", ItemType.Punchcard),
        new("Punchcard - PRPG Shell", "PRPGShell", ItemType.Punchcard),
        new("Punchcard - THRM Shell", "THRMShell", ItemType.Punchcard),
        new("Punchcard - ATMC Shell", "ATMCShell", ItemType.Punchcard),
        new("Punchcard - CYAN Shell", "CYANShell", ItemType.Punchcard),
        new("Punchcard - EQKE Shell", "EQKEShell", ItemType.Punchcard),
        new("Punchcard - Powder Charges", "PowderCharges", ItemType.Punchcard),
        new("Punchcard - Scout Plane", "ScoutPlane", ItemType.Punchcard),
        new("Punchcard - Emergency Move", "MoveZone", ItemType.Punchcard),
        new("Punchcard - Spotter", "Spotter", ItemType.Punchcard),
        new("Punchcard - Location Report", "LocationReport", ItemType.Punchcard),
        new("Requisition - Spotter", "SpawnSpotter", ItemType.Spawn),
        new("Requisition - Location Report", "SpawnLocationReport", ItemType.Spawn),
        new("Requisition - Powder Charges", "SpawnPowderCharges", ItemType.Spawn),
        new("Requisition - Requisition Points", "SpawnRequisitionPoints", ItemType.Spawn),
        new("Trap - Emergency Move", "TrapEmergencyMove", ItemType.Trap),
        new("Trap - Magazine Filler", "TrapFillMagazine", ItemType.Trap),
        new("Trap - Sabotage", "TrapSabotage", ItemType.Trap),
        new("Trap - Counter-Battery", "TrapCounterBattery", ItemType.Trap),
    };
    
    // indexing helpers
    public static readonly IReadOnlyDictionary<string, MissionInfo> MissionsByGameId = Missions.ToImmutableDictionary(m => m.GameId);
    public static readonly IReadOnlyDictionary<string, ItemInfo> ItemsByApName = Items.ToImmutableDictionary(i => i.APName);
    public static readonly IReadOnlyDictionary<string, ItemInfo> ItemsByGameId = Items.ToImmutableDictionary(i => i.GameId);

    public static readonly IReadOnlyDictionary<string, string> MedalLocationsByGameKey = (
        from mission in Missions
        from medal in mission.Medals
        from tier in new[] { "Bronze", "Silver", "Gold" }
        select (
            Key: $"{mission.GameId}-{medal.DisplayName}-{tier}",
            Ap: $"{mission.APName} - {medal.Abbreviation} {tier}"
        )
    ).ToImmutableDictionary(x => x.Key, x => x.Ap);
}