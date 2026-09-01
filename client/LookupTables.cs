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
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Austere Service Medal-Bronze", "Mission 4: Counter-Battery - AS Bronze");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Austere Service Medal-Silver", "Mission 4: Counter-Battery - AS Silver");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-Austere Service Medal-Gold", "Mission 4: Counter-Battery - AS Gold");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-No Quarter Cross-Bronze", "Mission 4: Counter-Battery - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-No Quarter Cross-Silver", "Mission 4: Counter-Battery - NQ Silver");
            MedalNameToAPLocationNameTable.Add("Artillery Introduction-No Quarter Cross-Gold", "Mission 4: Counter-Battery - NQ Gold");
            MedalNameToAPLocationNameTable.Add("IronRoad-Measured Fire Star-Bronze", "Mission 5: Iron Road - MF Bronze");
            MedalNameToAPLocationNameTable.Add("IronRoad-Measured Fire Star-Silver", "Mission 5: Iron Road - MF Silver");
            MedalNameToAPLocationNameTable.Add("IronRoad-Measured Fire Star-Gold", "Mission 5: Iron Road - MF Gold");
            MedalNameToAPLocationNameTable.Add("IronRoad-Unrelenting Volley Medal-Bronze", "Mission 5: Iron Road - UV Bronze");
            MedalNameToAPLocationNameTable.Add("IronRoad-Unrelenting Volley Medal-Silver", "Mission 5: Iron Road - UV Silver");
            MedalNameToAPLocationNameTable.Add("IronRoad-Unrelenting Volley Medal-Gold", "Mission 5: Iron Road - UV Gold");
            MedalNameToAPLocationNameTable.Add("IronRoad-Ordnance Efficiency Cross-Bronze", "Mission 5: Iron Road - OE Bronze");
            MedalNameToAPLocationNameTable.Add("IronRoad-Ordnance Efficiency Cross-Silver", "Mission 5: Iron Road - OE Silver");
            MedalNameToAPLocationNameTable.Add("IronRoad-Ordnance Efficiency Cross-Gold", "Mission 5: Iron Road - OE Gold");
            MedalNameToAPLocationNameTable.Add("IronRoad-Unaided Fire Distinction-Bronze", "Mission 5: Iron Road - UF Bronze");
            MedalNameToAPLocationNameTable.Add("IronRoad-Unaided Fire Distinction-Silver", "Mission 5: Iron Road - UF Silver");
            MedalNameToAPLocationNameTable.Add("IronRoad-Unaided Fire Distinction-Gold", "Mission 5: Iron Road - UF Gold");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Measured Fire Star-Bronze", "Mission 6: Siege of Cartagena - MF Bronze");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Measured Fire Star-Silver", "Mission 6: Siege of Cartagena - MF Silver");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Measured Fire Star-Gold", "Mission 6: Siege of Cartagena - MF Gold");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-No Quarter Cross-Bronze", "Mission 6: Siege of Cartagena - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-No Quarter Cross-Silver", "Mission 6: Siege of Cartagena - NQ Silver");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-No Quarter Cross-Gold", "Mission 6: Siege of Cartagena - NQ Gold");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Unaided Fire Distinction-Bronze", "Mission 6: Siege of Cartagena - UF Bronze");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Unaided Fire Distinction-Silver", "Mission 6: Siege of Cartagena - UF Silver");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Unaided Fire Distinction-Gold", "Mission 6: Siege of Cartagena - UF Gold");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Austere Service Medal-Bronze", "Mission 6: Siege of Cartagena - AS Bronze");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Austere Service Medal-Silver", "Mission 6: Siege of Cartagena - AS Silver");
            MedalNameToAPLocationNameTable.Add("SiegeOfCartagena-Austere Service Medal-Gold", "Mission 6: Siege of Cartagena - AS Gold");
            MedalNameToAPLocationNameTable.Add("The Gorge-Measured Fire Star-Bronze", "Mission 7: The Gorge - MF Bronze");
            MedalNameToAPLocationNameTable.Add("The Gorge-Measured Fire Star-Silver", "Mission 7: The Gorge - MF Silver");
            MedalNameToAPLocationNameTable.Add("The Gorge-Measured Fire Star-Gold", "Mission 7: The Gorge - MF Gold");
            MedalNameToAPLocationNameTable.Add("The Gorge-Unrelenting Volley Medal-Bronze", "Mission 7: The Gorge - UV Bronze");
            MedalNameToAPLocationNameTable.Add("The Gorge-Unrelenting Volley Medal-Silver", "Mission 7: The Gorge - UV Silver");
            MedalNameToAPLocationNameTable.Add("The Gorge-Unrelenting Volley Medal-Gold", "Mission 7: The Gorge - UV Gold");
            MedalNameToAPLocationNameTable.Add("The Gorge-No Quarter Cross-Bronze", "Mission 7: The Gorge - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("The Gorge-No Quarter Cross-Silver", "Mission 7: The Gorge - NQ Silver");
            MedalNameToAPLocationNameTable.Add("The Gorge-No Quarter Cross-Gold", "Mission 7: The Gorge - NQ Gold");
            MedalNameToAPLocationNameTable.Add("The Gorge-Ordnance Efficiency Cross-Bronze", "Mission 7: The Gorge - OE Bronze");
            MedalNameToAPLocationNameTable.Add("The Gorge-Ordnance Efficiency Cross-Silver", "Mission 7: The Gorge - OE Silver");
            MedalNameToAPLocationNameTable.Add("The Gorge-Ordnance Efficiency Cross-Gold", "Mission 7: The Gorge - OE Gold");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Measured Fire Star-Bronze", "Mission 8: Rock of Gibraltar - MF Bronze");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Measured Fire Star-Silver", "Mission 8: Rock of Gibraltar - MF Silver");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Measured Fire Star-Gold", "Mission 8: Rock of Gibraltar - MF Gold");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-No Quarter Cross-Bronze", "Mission 8: Rock of Gibraltar - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-No Quarter Cross-Silver", "Mission 8: Rock of Gibraltar - NQ Silver");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-No Quarter Cross-Gold", "Mission 8: Rock of Gibraltar - NQ Gold");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Unaided Fire Distinction-Bronze", "Mission 8: Rock of Gibraltar - UF Bronze");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Unaided Fire Distinction-Silver", "Mission 8: Rock of Gibraltar - UF Silver");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Unaided Fire Distinction-Gold", "Mission 8: Rock of Gibraltar - UF Gold");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Austere Service Medal-Bronze", "Mission 8: Rock of Gibraltar - AS Bronze");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Austere Service Medal-Silver", "Mission 8: Rock of Gibraltar - AS Silver");
            MedalNameToAPLocationNameTable.Add("RockofGibraltar-Austere Service Medal-Gold", "Mission 8: Rock of Gibraltar - AS Gold");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Measured Fire Star-Bronze", "Mission 9: Dead Reckoning - MF Bronze");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Measured Fire Star-Silver", "Mission 9: Dead Reckoning - MF Silver");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Measured Fire Star-Gold", "Mission 9: Dead Reckoning - MF Gold");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Unbroken Volley Medal-Bronze", "Mission 9: Dead Reckoning - UV Bronze");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Unbroken Volley Medal-Silver", "Mission 9: Dead Reckoning - UV Silver");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Unbroken Volley Medal-Gold", "Mission 9: Dead Reckoning - UV Gold");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-No Quarter Cross-Bronze", "Mission 9: Dead Reckoning - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-No Quarter Cross-Silver", "Mission 9: Dead Reckoning - NQ Silver");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-No Quarter Cross-Gold", "Mission 9: Dead Reckoning - NQ Gold");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Austere Service Medal-Bronze", "Mission 9: Dead Reckoning - AS Bronze");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Austere Service Medal-Silver", "Mission 9: Dead Reckoning - AS Silver");
            MedalNameToAPLocationNameTable.Add("DeadReckoning-Austere Service Medal-Gold", "Mission 9: Dead Reckoning - AS Gold");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Measured Fire Star-Bronze", "Mission 10: Fire on Call - MF Bronze");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Measured Fire Star-Silver", "Mission 10: Fire on Call - MF Silver");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Measured Fire Star-Gold", "Mission 10: Fire on Call - MF Gold");
            MedalNameToAPLocationNameTable.Add("FireOnCall-No Quarter Cross-Bronze", "Mission 10: Fire on Call - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("FireOnCall-No Quarter Cross-Silver", "Mission 10: Fire on Call - NQ Silver");
            MedalNameToAPLocationNameTable.Add("FireOnCall-No Quarter Cross-Gold", "Mission 10: Fire on Call - NQ Gold");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Ordnance Efficiency Cross-Bronze", "Mission 10: Fire on Call - OE Bronze");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Ordnance Efficiency Cross-Silver", "Mission 10: Fire on Call - OE Silver");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Ordnance Efficiency Cross-Gold", "Mission 10: Fire on Call - OE Gold");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Unaided Fire Distinction-Bronze", "Mission 10: Fire on Call - UF Bronze");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Unaided Fire Distinction-Silver", "Mission 10: Fire on Call - UF Silver");
            MedalNameToAPLocationNameTable.Add("FireOnCall-Unaided Fire Distinction-Gold", "Mission 10: Fire on Call - UF Gold");
            MedalNameToAPLocationNameTable.Add("HighTide-Unbroken Volley Medal-Bronze", "Mission 11: High Tide - UV Bronze");
            MedalNameToAPLocationNameTable.Add("HighTide-Unbroken Volley Medal-Silver", "Mission 11: High Tide - UV Silver");
            MedalNameToAPLocationNameTable.Add("HighTide-Unbroken Volley Medal-Gold", "Mission 11: High Tide - UV Gold");
            MedalNameToAPLocationNameTable.Add("HighTide-Salvo Commendation-Bronze", "Mission 11: High Tide - SC Bronze");
            MedalNameToAPLocationNameTable.Add("HighTide-Salvo Commendation-Silver", "Mission 11: High Tide - SC Silver");
            MedalNameToAPLocationNameTable.Add("HighTide-Salvo Commendation-Gold", "Mission 11: High Tide - SC Gold");
            MedalNameToAPLocationNameTable.Add("HighTide-Marksman's Cross-Bronze", "Mission 11: High Tide - MC Bronze");
            MedalNameToAPLocationNameTable.Add("HighTide-Marksman's Cross-Silver", "Mission 11: High Tide - MC Silver");
            MedalNameToAPLocationNameTable.Add("HighTide-Marksman's Cross-Gold", "Mission 11: High Tide - MC Gold");
            MedalNameToAPLocationNameTable.Add("BlindFire-Austere Service Medal-Bronze", "Mission 12: Blind Fire - AS Bronze");
            MedalNameToAPLocationNameTable.Add("BlindFire-Austere Service Medal-Silver", "Mission 12: Blind Fire - AS Silver");
            MedalNameToAPLocationNameTable.Add("BlindFire-Austere Service Medal-Gold", "Mission 12: Blind Fire - AS Gold");
            MedalNameToAPLocationNameTable.Add("BlindFire-Unbroken Volley Medal-Bronze", "Mission 12: Blind Fire - UV Bronze");
            MedalNameToAPLocationNameTable.Add("BlindFire-Unbroken Volley Medal-Silver", "Mission 12: Blind Fire - UV Silver");
            MedalNameToAPLocationNameTable.Add("BlindFire-Unbroken Volley Medal-Gold", "Mission 12: Blind Fire - UV Gold");
            MedalNameToAPLocationNameTable.Add("BlindFire-No Quarter Cross-Bronze", "Mission 12: Blind Fire - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("BlindFire-No Quarter Cross-Silver", "Mission 12: Blind Fire - NQ Silver");
            MedalNameToAPLocationNameTable.Add("BlindFire-No Quarter Cross-Gold", "Mission 12: Blind Fire - NQ Gold");
            MedalNameToAPLocationNameTable.Add("BlindFire-Unaided Fire Distinction-Bronze", "Mission 12: Blind Fire - UF Bronze");
            MedalNameToAPLocationNameTable.Add("BlindFire-Unaided Fire Distinction-Silver", "Mission 12: Blind Fire - UF Silver");
            MedalNameToAPLocationNameTable.Add("BlindFire-Unaided Fire Distinction-Gold", "Mission 12: Blind Fire - UF Gold");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Unaided Fire Distinction-Bronze", "Mission 13: Phantom Battery - UF Bronze");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Unaided Fire Distinction-Silver", "Mission 13: Phantom Battery - UF Silver");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Unaided Fire Distinction-Gold", "Mission 13: Phantom Battery - UF Gold");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-No Quarter Cross-Bronze", "Mission 13: Phantom Battery - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-No Quarter Cross-Silver", "Mission 13: Phantom Battery - NQ Silver");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-No Quarter Cross-Gold", "Mission 13: Phantom Battery - NQ Gold");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Marksman's Cross-Bronze", "Mission 13: Phantom Battery - MC Bronze");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Marksman's Cross-Silver", "Mission 13: Phantom Battery - MC Silver");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Marksman's Cross-Gold", "Mission 13: Phantom Battery - MC Gold");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Counter-Battery Commendation-Bronze", "Mission 13: Phantom Battery - CB Bronze");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Counter-Battery Commendation-Silver", "Mission 13: Phantom Battery - CB Silver");
            MedalNameToAPLocationNameTable.Add("PhantomBattery-Counter-Battery Commendation-Gold", "Mission 13: Phantom Battery - CB Gold");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Measured Fire Star-Bronze", "Mission 14: Final Harvest - MF Bronze");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Measured Fire Star-Silver", "Mission 14: Final Harvest - MF Silver");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Measured Fire Star-Gold", "Mission 14: Final Harvest - MF Gold");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Unaided Fire Distinction-Bronze", "Mission 14: Final Harvest - UF Bronze");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Unaided Fire Distinction-Silver", "Mission 14: Final Harvest - UF Silver");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Unaided Fire Distinction-Gold", "Mission 14: Final Harvest - UF Gold");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-No Quarter Cross-Bronze", "Mission 14: Final Harvest - NQ Bronze");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-No Quarter Cross-Silver", "Mission 14: Final Harvest - NQ Silver");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-No Quarter Cross-Gold", "Mission 14: Final Harvest - NQ Gold");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Rapid Engagement Medal-Bronze", "Mission 14: Final Harvest - RE Bronze");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Rapid Engagement Medal-Silver", "Mission 14: Final Harvest - RE Silver");
            MedalNameToAPLocationNameTable.Add("FinalHarvest-Rapid Engagement Medal-Gold", "Mission 14: Final Harvest - RE Gold");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 1-Bronze", "Mission 15: White Shells - E1 Bronze");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 1-Silver", "Mission 15: White Shells - E1 Silver");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 1-Gold", "Mission 15: White Shells - E1 Gold");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 2-Bronze", "Mission 15: White Shells - E2 Bronze");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 2-Silver", "Mission 15: White Shells - E2 Silver");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 2-Gold", "Mission 15: White Shells - E2 Gold");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 3-Bronze", "Mission 15: White Shells - E3 Bronze");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 3-Silver", "Mission 15: White Shells - E3 Silver");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 3-Gold", "Mission 15: White Shells - E3 Gold");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 4-Bronze", "Mission 15: White Shells - E4 Bronze");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 4-Silver", "Mission 15: White Shells - E4 Silver");
            MedalNameToAPLocationNameTable.Add("WhiteShells-Ending 4-Gold", "Mission 15: White Shells - E4 Gold");
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
            
            List<string> mission6List = new();
            mission6List.Add("SMKShell");

            List<string> mission9List = new();
            mission9List.Add("TEARShell");

            List<string> mission13List = new();
            mission13List.Add("MoveZone");
            mission13List.Add("ScoutPlane");

            List<string> mission15List = new();
            mission15List.Add("ATMCShell");
            
            MissionPrerequisiteTable.Add("ceremony and HCHE",  mission2List);
            MissionPrerequisiteTable.Add("Insurrections and Requisitions",  mission3List);
            MissionPrerequisiteTable.Add("SiegeOfCartagena",  mission6List);
            MissionPrerequisiteTable.Add("DeadReckoning",  mission9List);
            MissionPrerequisiteTable.Add("PhantomBattery",  mission13List);
            MissionPrerequisiteTable.Add("WhiteShells",  mission15List);
            
            return;
        }
    }
}