using System;
using System.Collections.Generic;
using APNestClient.ModLoader;

namespace APNestClient.Util;

public static class MissionUtil
{
    public static Zone RandomZone(EntityRoles role)
    {
        Random rand = new();
        
        List<Zone> zones = new();
        foreach (Zone zone in MissionManager.Instance.CurrentMission.Zones)
        {
            if (zone.Role == role)
            {
                zones.Add(zone);
            }
        }

        if (zones.Count == 0)
        {
            Logger.Warning("Could not  find any zones for role " + role + ". Using fallback zone.");
            zones.Add(MissionManager.Instance.CurrentMission.Zones[0]);
        }

        return zones[rand.Next(zones.Count)];
    }
}