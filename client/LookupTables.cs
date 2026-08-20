using System.Collections.Generic;

namespace APNestClient;

public class LookupTables
{
    public Dictionary<string, string> MissionNameToAPLocationNameTable = new();
    public Dictionary<long, string> ApItemIdToGameId = new();
    
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
            ApItemIdToGameId.Add(9140001, "HEShell");
            ApItemIdToGameId.Add(9140002, "STARShell");
            ApItemIdToGameId.Add(9140003, "APShell");
            ApItemIdToGameId.Add(9140004, "SMOKEShell");
            ApItemIdToGameId.Add(9140005, "TEARShell");
            ApItemIdToGameId.Add(9140006, "INCNShell");
            ApItemIdToGameId.Add(9140007, "HCHEShell");
            ApItemIdToGameId.Add(9140008, "DRILShell");
            ApItemIdToGameId.Add(9140009, "LEShell");
            ApItemIdToGameId.Add(9140010, "PHGNShell");
            ApItemIdToGameId.Add(9140011, "WPShell");
            ApItemIdToGameId.Add(9140012, "PCLMShell");
            ApItemIdToGameId.Add(9140013, "APHEShell");
            ApItemIdToGameId.Add(9140014, "FLCHShell");
            ApItemIdToGameId.Add(9140015, "PRPGShell");
            ApItemIdToGameId.Add(9140016, "THRMShell");
            ApItemIdToGameId.Add(9140017, "ATMCShell");
            ApItemIdToGameId.Add(9140018, "CYANShell");
            ApItemIdToGameId.Add(9140019, "EQKEShell");
            ApItemIdToGameId.Add(9140020, "PowderCharges");
            ApItemIdToGameId.Add(9140021, "ScoutPlane");
            ApItemIdToGameId.Add(9140022, "MoveZone");
            // useful
            ApItemIdToGameId.Add(9141001, "Spotter");
            ApItemIdToGameId.Add(9141002, "SpawnSpotter");
            ApItemIdToGameId.Add(9141003, "LocationReport");
            ApItemIdToGameId.Add(9141004, "SpawnLocationReport");
            //filler
            ApItemIdToGameId.Add(9142001, "SpawnPowderCharges");
            ApItemIdToGameId.Add(9142002, "SpawnRequisitionPoints");
            // traps
            ApItemIdToGameId.Add(9143001, "TrapEmergencyMove");
            ApItemIdToGameId.Add(9143002, "TrapFillMagazine");
            ApItemIdToGameId.Add(9143003, "TrapSabotage");
            ApItemIdToGameId.Add(9143004, "TrapCounterBattery");
            return;
        }
    }
}