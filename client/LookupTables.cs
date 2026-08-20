using System.Collections.Generic;

namespace APNestClient;

public class LookupTables
{
    public Dictionary<string, string> MissionNameToAPLocationNameTable = new();
    public Dictionary<string, string> ApItemNameToGameId = new();
    
    public enum TableType
    {
        Locations,
        Items,
    }

    public LookupTables(TableType tableType)
    {
        if (tableType == TableType.Locations)
        {
            MissionNameToAPLocationNameTable.Add("Hospital False Flag", "Mission 1: Calibration Fire");
            MissionNameToAPLocationNameTable.Add("ceremony and HCHE", "Mission 2: Fire and Light");
            MissionNameToAPLocationNameTable.Add("Insurrections and Requisitions", "Mission 3: Liberation");
            MissionNameToAPLocationNameTable.Add("Artillery Introduction", "Mission 4: Counter-Battery");
            MissionNameToAPLocationNameTable.Add("IronRoad", "Mission 5: Iron Road");
            MissionNameToAPLocationNameTable.Add("SiegeOfCartagena", "Mission 6: Siege of Cartagena");
            MissionNameToAPLocationNameTable.Add("The Gorge", "Mission 7: The Gorge");
            MissionNameToAPLocationNameTable.Add("RockofGibraltar", "Mission 8: Rock of Gibraltar");
            MissionNameToAPLocationNameTable.Add("DeadReckoning", "Mission 9: Dead Reckoning");
            MissionNameToAPLocationNameTable.Add("FireOnCall", "Mission 10: Fire on Call");
            MissionNameToAPLocationNameTable.Add("HighTide", "Mission 11: High Tide");
            MissionNameToAPLocationNameTable.Add("BlindFire", "Mission 12: Blind Fire");
            MissionNameToAPLocationNameTable.Add("PhantomBattery", "Mission 13: Phantom Battery");
            MissionNameToAPLocationNameTable.Add("FinalHarvest", "Mission 14: Final Harvest");
            MissionNameToAPLocationNameTable.Add("WhiteShells", "Mission 15: White Shells");
            return;
        }
        if (tableType == TableType.Items)
        {
            // progression
            ApItemNameToGameId.Add("Punchcard - HE Shell", "HEShell");
            ApItemNameToGameId.Add("Punchcard - STAR Shell", "STARShell");
            ApItemNameToGameId.Add("Punchcard - AP Shell", "APShell");
            ApItemNameToGameId.Add("Punchcard - SMK Shell", "SMOKEShell");
            ApItemNameToGameId.Add("Punchcard - TEAR Shell", "TEARShell");
            ApItemNameToGameId.Add("Punchcard - INCN Shell", "INCNShell");
            ApItemNameToGameId.Add("Punchcard - HCHE Shell", "HCHEShell");
            ApItemNameToGameId.Add("Punchcard - DRIL Shell", "DRILShell");
            ApItemNameToGameId.Add("Punchcard - LE Shell", "LEShell");
            ApItemNameToGameId.Add("Punchcard - PHGN Shell", "PHGNShell");
            ApItemNameToGameId.Add("Punchcard - WP Shell", "WPShell");
            ApItemNameToGameId.Add("Punchcard - PCLM Shell", "PCLMShell");
            ApItemNameToGameId.Add("Punchcard - APHE Shell", "APHEShell");
            ApItemNameToGameId.Add("Punchcard - FLCH Shell", "FLCHShell");
            ApItemNameToGameId.Add("Punchcard - PRPG Shell", "PRPGShell");
            ApItemNameToGameId.Add("Punchcard - THRM Shell", "THRMShell");
            ApItemNameToGameId.Add("Punchcard - ATMC Shell", "ATMCShell");
            ApItemNameToGameId.Add("Punchcard - CYAN Shell", "CYANShell");
            ApItemNameToGameId.Add("Punchcard - EQKE Shell", "EQKEShell");
            ApItemNameToGameId.Add("Punchcard - Powder Charges", "PowderCharges");
            ApItemNameToGameId.Add("Punchcard - Scout Plane", "ScoutPlane");
            ApItemNameToGameId.Add("Punchcard - Emergency Move", "MoveZone");
            // useful
            ApItemNameToGameId.Add("Punchcard - Spotter", "Spotter");
            ApItemNameToGameId.Add("Requisition - Spotter", "SpawnSpotter");
            ApItemNameToGameId.Add("Punchcard - Location Report", "LocationReport");
            ApItemNameToGameId.Add("Requisition - Location Report", "SpawnLocationReport");
            //filler
            ApItemNameToGameId.Add("Requisition - Powder Charges", "SpawnPowderCharges");
            ApItemNameToGameId.Add("Requisition - Requisition Points", "SpawnRequisitionPoints");
            // traps
            ApItemNameToGameId.Add("Trap - Emergency Move", "TrapEmergencyMove");
            ApItemNameToGameId.Add("Trap - Magazine Filler", "TrapFillMagazine");
            ApItemNameToGameId.Add("Trap - Sabotage", "TrapSabotage");
            ApItemNameToGameId.Add("Trap - Counter-Battery", "TrapCounterBattery");
            return;
        }
    }
}