using System.Collections.Generic;

namespace APNestClient;

public class LookupTables
{
    public Dictionary<string, string> MissionNameToAPLocationNameTable = new();
    public Dictionary<string, string> MedalNameToAPLocationNameTable = new();
    public Dictionary<string, string> ApItemNameToGameIdTable = new();
    public Dictionary<string, List<string>> MissionPrerequisiteTable = new();
    
    public enum TableType
    {
        MissionLocations,
        MedalLocations,
        Items,
        MissionPrerequisites,
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
            MedalNameToAPLocationNameTable.Add("Hospital False Flag-Ordnance Efficiency Laurel-Bronze", "Mission 1: Calibration Fire - OE Bronze");
            MedalNameToAPLocationNameTable.Add("Hospital False Flag-Ordnance Efficiency Laurel-Silver", "Mission 1: Calibration Fire - OE Silver");
            MedalNameToAPLocationNameTable.Add("Hospital False Flag-Ordnance Efficiency Laurel-Gold", "Mission 1: Calibration Fire - OE Gold");
            MedalNameToAPLocationNameTable.Add("ceremony and HCHE-Ordnance Efficiency Citation-Bronze", "Mission 2: Fire and Light - OE Bronze");
            MedalNameToAPLocationNameTable.Add("ceremony and HCHE-Ordnance Efficiency Citation-Silver", "Mission 2: Fire and Light - OE Silver");
            MedalNameToAPLocationNameTable.Add("ceremony and HCHE-Ordnance Efficiency Citation-Gold", "Mission 2: Fire and Light - OE Gold");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Measured Fire Star-Bronze", "Mission 3: Liberation - MF Bronze");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Measured Fire Star-Silver", "Mission 3: Liberation - MF Silver");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Measured Fire Star-Gold", "Mission 3: Liberation - MF Gold");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Marksman's Cross-Bronze", "Mission 3: Liberation - MC Bronze");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Marksman's Cross-Silver", "Mission 3: Liberation - MC Silver");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Marksman's Cross-Gold", "Mission 3: Liberation - MC Gold");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Unbroken Volley Medal-Bronze", "Mission 3: Liberation - UV Bronze");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Unbroken Volley Medal-Silver", "Mission 3: Liberation - UV Silver");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Unbroken Volley Medal-Gold", "Mission 3: Liberation - UV Gold");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Austere Service Medal-Bronze", "Mission 3: Liberation - AS Bronze");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Austere Service Medal-Silver", "Mission 3: Liberation - AS Silver");
            MedalNameToAPLocationNameTable.Add("Insurrections and Requisitions-Austere Service Medal-Gold", "Mission 3: Liberation - AS Gold");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Salvo Commendation-Bronze", "Mission 4: Counter-Battery - SC Bronze");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Salvo Commendation-Silver", "Mission 4: Counter-Battery - SC Silver");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Salvo Commendation-Gold", "Mission 4: Counter-Battery - SC Gold");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Counter-Battery Commendation-Bronze", "Mission 4: Counter-Battery - CB Bronze");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Counter-Battery Commendation-Silver", "Mission 4: Counter-Battery - CB Silver");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Counter-Battery Commendation-Gold", "Mission 4: Counter-Battery - CB Gold");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Counter-Austere Service Medal-Bronze", "Mission 4: Counter-Battery - AS Bronze");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Counter-Austere Service Medal-Silver", "Mission 4: Counter-Battery - AS Silver");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Counter-Austere Service Medal-Gold", "Mission 4: Counter-Battery - AS Gold");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-No Quarter Cross-Bronze", "Mission 4: Counter-Battery - AS Bronze");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-No Quarter Cross-Silver", "Mission 4: Counter-Battery - AS Silver");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-No Quarter Cross-Gold", "Mission 4: Counter-Battery - AS Gold");
            return;
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

        if (tableType == TableType.MissionPrerequisites)
        {
            List<string> mission2List = new();
            mission2List.Add("HEShell");
            mission2List.Add("STARShell");

            List<string> mission3List = new();
            mission3List.Add("APShell");
            mission3List.Add("PowderCharges");

            List<string> mission9List = new();
            mission9List.Add("TEARShell");

            List<string> mission13List = new();
            mission13List.Add("MoveZone");

            List<string> mission15List = new();
            mission15List.Add("ATMCShell");
            
            MissionPrerequisiteTable.Add("ceremony and HCHE",  mission2List);
            MissionPrerequisiteTable.Add("Insurrections and Requisitions",  mission3List);
            MissionPrerequisiteTable.Add("DeadReckoning",  mission9List);
            MissionPrerequisiteTable.Add("PhantomBattery",  mission13List);
            MissionPrerequisiteTable.Add("WhiteShells",  mission15List);
            
            return;
        }
    }
}