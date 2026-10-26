namespace APNestClient;

public class TutorialGuard
{
    public static bool IsTutorial()
    {
        return (
            MissionManager.Instance == null ||
            MissionManager.Instance.CurrentMission == null ||
            // Most of the game's features required for this mod (like CBT or MissionFailures)
            // are not available in the first two missions
            MissionManager.Instance.CurrentMission.MissionID == "Hospital False Flag" ||
            MissionManager.Instance.CurrentMission.MissionID == "ceremony and HCHE"
        );
    }
}