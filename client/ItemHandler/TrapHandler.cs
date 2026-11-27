using System;
using APNestClient.Util;
using Il2CppInterop.Runtime;
using UnityEngine;
using Logger = APNestClient.ModLoader.Logger;
using Random = UnityEngine.Random;

namespace APNestClient.ItemHandler;

public static class TrapHandler
{
    public static void HandleMagazineTrap()
    {
        PunchcardDefinitionV2 starShellCard = GraphUtil.GetPunchcardDefinitionById("STARShell");
        ShellDefinition starShell = GraphUtil.FindFirstNode<ShellDefinition>(starShellCard);
        if (starShell == null)
        {
            Logger.Warning("Could not resolve Shell 'STARShell' for magazine fill trap.");
            return;
        }
        
        ShellSlotPool shellSlotPool = UnityEngine.Object.FindFirstObjectByType<ShellSlotPool>();
        try
        {
            while (
                shellSlotPool.InsertShell(
                    starShell,
                    ShellSlotPool.ShellInsertionMode.FillOneThenNext,
                    ShellSlotPool.ShellSource.Punchcard,
                    out _,
                    out _
                )
            ) {}
        }
        catch (Il2CppException)
        {
            Logger.Msg(("Magazine fill trap complete."));
        }
    }

    public static void HandleEmergencyMoveTrap()
    {
        PunchcardDefinitionV2 emergencyMoveCard = GraphUtil.GetPunchcardDefinitionById("MoveZone");
        LocationSelection moveTarget = GraphUtil.FindFirstNode<State_MoveTurret>(emergencyMoveCard).LocationToMoveTo;

        if (moveTarget == null)
        {
            Logger.Warning("Could not resolve move target for emergency move trap.");
            return;
        }

        if (
            moveTarget.LocationType == LocationSelection.LocationTypes.Relative
            && moveTarget.RelativeTo == LocationSelection.RelativeReferenceTypes.Self
            && moveTarget.RelativeDirection == LocationSelection.RelativeDirections.RandomInRadius
        )
        {
            float minDistance = moveTarget.DistanceMin.Get(null);
            float maxDistance = moveTarget.DistanceMax.Get(null);
            float distance = Random.Range(minDistance, maxDistance);
            
            float direction = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector3 currentPos = TurretController.Instance.transform.position;
            Vector3 offset = new Vector3(Mathf.Cos(direction) * distance, 0f, Mathf.Sin(direction) * distance);
            Vector3 targetPos =
                GridReference.ClampToGridBounds(currentPos + offset, FireMission.Instance.GetGridBounds());
            
            TurretController.Instance.MoveTurret(targetPos);
            
            GraphUtil.SubmitTeleprinterReport(emergencyMoveCard, "");
            
            return;
        }
        
        Logger.Warning(
            "Could not resolve relative move target for emergency move trap. Moving to random allied position."
        );

        Zone targetZone = MissionUtil.RandomZone(EntityRoles.Ally);
        Il2CppSystem.Random moveRand = new();
        GridReference moveGridRef = targetZone.GetRandomGridPosition(moveRand);
        Vector3 movePos = moveGridRef.GetLocation(FireMission.Instance.GetGridBounds());

        TurretController.Instance.MoveTurret(movePos);

        GraphUtil.SubmitTeleprinterReport(emergencyMoveCard, "");
    }

    public void HandleCounterBatteryTrap()
    {
        
    }
}