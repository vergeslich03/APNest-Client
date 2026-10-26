using UnityEngine;
using Logger = APNestClient.ModLoader.Logger;

namespace APNestClient;

public class CBHelper
{
    // Returns the live CounterBatteryTimer for the current mission, spawning it if the mission has a
    // State_StartTimer node but its in-mission trigger hasn't fired yet. Returns null
    // if there's no such node at all this mission (e.g. missions 1/3).
    public static CounterBatteryTimer GetOrSpawnCbtTimer()
    {
        if (CounterBatteryTimer.Instance != null)
        {
            return CounterBatteryTimer.Instance;
        }

        if (MissionManager.Instance == null || MissionManager.Instance.CurrentMission == null)
        {
            return null;
        }

        State_StartTimer startTimerNode = null;
        foreach (Node node in MissionManager.Instance.CurrentMission.nodes)
        {
            startTimerNode = node.TryCast<State_StartTimer>();
            if (startTimerNode != null)
            {
                break;
            }
        }

        // Mission 3 has no State_StartTimer node of its own, so I borrow one from another mission's graph
        if (startTimerNode == null && MissionManager.Instance.CurrentOperation != null)
        {
            foreach (MissionNode missionNode in MissionManager.Instance.CurrentOperation.Missions)
            {
                if (missionNode.Mission == null || missionNode.Mission == MissionManager.Instance.CurrentMission)
                {
                    continue;
                }

                foreach (Node node in missionNode.Mission.nodes)
                {
                    startTimerNode = node.TryCast<State_StartTimer>();
                    if (startTimerNode != null)
                    {
                        break;
                    }
                }

                if (startTimerNode != null)
                {
                    Logger.Msg("GetOrSpawnCbtTimer: borrowing State_StartTimer from mission '" + missionNode.Mission.MissionID + "'");
                    break;
                }
            }
        }

        if (startTimerNode == null || startTimerNode.Prefab_BatteryTimer == null)
        {
            return null;
        }

        CounterBatteryTimer spawned = UnityEngine.Object.Instantiate(startTimerNode.Prefab_BatteryTimer);
        spawned.Init(startTimerNode.InitalTime);

        // Missions without a native CB encounter (e.g. mission 3) still have a static,
        // closed "Timer Hatch" prop baked into the base scene decorating the empty mount.
        // My spawned prefab brings its own hatch to the same slot, so disable the static one to stop the two from
        // overlapping/clipping into each other.
        foreach (Transform t in UnityEngine.Object.FindObjectsOfType<Transform>(true))
        {
            if (t.gameObject.name == "Timer Hatch" && t.parent == null)
            {
                t.gameObject.SetActive(false);
                break;
            }
        }

        return spawned;
    }
}