using System.Collections.Generic;

namespace APNestClient;

public class LookupTables
{
    public Dictionary<string, string> MissionNameToAPLocationNameTable = new();
    public Dictionary<string, string> MedalNameToAPLocationNameTable = new();
    public Dictionary<string, string> ApItemNameToGameIdTable = new();
    
    public enum TableType
    {
        MissionLocations,
        MedalLocations,
        Items,
    }

    public LookupTables(TableType tableType)
    {
        if (tableType == TableType.MissionLocations)
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
        if (tableType == TableType.MedalLocations)
        {
            MedalNameToAPLocationNameTable.Add("Hospital False Flag-shots_fired_1to3-Bronze", "Mission 1: Calibration Fire - OE Bronze");
            MedalNameToAPLocationNameTable.Add("Hospital False Flag-shots_fired_1to3-Silver", "Mission 1: Calibration Fire - OE Silver");
            MedalNameToAPLocationNameTable.Add("Hospital False Flag-shots_fired_1to3-Gold", "Mission 1: Calibration Fire - OE Gold");
        }
        if (tableType == TableType.Items)
        {
            // progression
            ApItemNameToGameIdTable.Add("Punchcard - HE Shell", "HEShell");
            ApItemNameToGameIdTable.Add("Punchcard - STAR Shell", "STARShell");
            ApItemNameToGameIdTable.Add("Punchcard - AP Shell", "APShell");
            ApItemNameToGameIdTable.Add("Punchcard - SMK Shell", "SMOKEShell");
            ApItemNameToGameIdTable.Add("Punchcard - TEAR Shell", "TEARShell");
            ApItemNameToGameIdTable.Add("Punchcard - INCN Shell", "INCNShell");
            ApItemNameToGameIdTable.Add("Punchcard - HCHE Shell", "HCHEShell");
            ApItemNameToGameIdTable.Add("Punchcard - DRIL Shell", "DRILShell");
            ApItemNameToGameIdTable.Add("Punchcard - LE Shell", "LEShell");
            ApItemNameToGameIdTable.Add("Punchcard - PHGN Shell", "PHGNShell");
            ApItemNameToGameIdTable.Add("Punchcard - WP Shell", "WPShell");
            ApItemNameToGameIdTable.Add("Punchcard - PCLM Shell", "PCLMShell");
            ApItemNameToGameIdTable.Add("Punchcard - APHE Shell", "APHEShell");
            ApItemNameToGameIdTable.Add("Punchcard - FLCH Shell", "FLCHShell");
            ApItemNameToGameIdTable.Add("Punchcard - PRPG Shell", "PRPGShell");
            ApItemNameToGameIdTable.Add("Punchcard - THRM Shell", "THRMShell");
            ApItemNameToGameIdTable.Add("Punchcard - ATMC Shell", "ATMCShell");
            ApItemNameToGameIdTable.Add("Punchcard - CYAN Shell", "CYANShell");
            ApItemNameToGameIdTable.Add("Punchcard - EQKE Shell", "EQKEShell");
            ApItemNameToGameIdTable.Add("Punchcard - Powder Charges", "PowderCharges");
            ApItemNameToGameIdTable.Add("Punchcard - Scout Plane", "ScoutPlane");
            ApItemNameToGameIdTable.Add("Punchcard - Emergency Move", "MoveZone");
            // useful
            ApItemNameToGameIdTable.Add("Punchcard - Spotter", "Spotter");
            ApItemNameToGameIdTable.Add("Requisition - Spotter", "SpawnSpotter");
            ApItemNameToGameIdTable.Add("Punchcard - Location Report", "LocationReport");
            ApItemNameToGameIdTable.Add("Requisition - Location Report", "SpawnLocationReport");
            //filler
            ApItemNameToGameIdTable.Add("Requisition - Powder Charges", "SpawnPowderCharges");
            ApItemNameToGameIdTable.Add("Requisition - Requisition Points", "SpawnRequisitionPoints");
            // traps
            ApItemNameToGameIdTable.Add("Trap - Emergency Move", "TrapEmergencyMove");
            ApItemNameToGameIdTable.Add("Trap - Magazine Filler", "TrapFillMagazine");
            ApItemNameToGameIdTable.Add("Trap - Sabotage", "TrapSabotage");
            ApItemNameToGameIdTable.Add("Trap - Counter-Battery", "TrapCounterBattery");
            return;
        }
    }
}