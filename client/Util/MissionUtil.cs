using System.Collections.Generic;
using APNestClient.ModLoader;

namespace APNestClient.Util;

public static class MissionUtil
{
    public static Zone RandomZone(EntityRoles role)
    {
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
            return MissionManager.Instance.CurrentMission.Zones[0];
        }

        return zones[0];
    }
}