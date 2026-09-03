using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace APNestClient;

[HarmonyPatch(typeof(EntityLocation), "OnEntityStateChanged")]
public class CBTimerStopHandler
{
    public static void Postfix(EntityLocation __instance, MapEntityStates oldState, MapEntityStates newState)
    {
        if (!newState.Has(MapEntityStates.Destroyed))
        {
            return;
        }

        if (!__instance.Entity.Role.Has(EntityRoles.Artillery) || !__instance.Entity.Role.Has(EntityRoles.Enemy))
        {
            return;
        }

        foreach (MapEntity mapEntity in FireMission.Instance.Entities.Values)
        {
            if (mapEntity.Role.Has(EntityRoles.Artillery) && mapEntity.Role.Has(EntityRoles.Enemy))
            {
                if (mapEntity.IsAlive)
                {
                    return;
                }
                
                CounterBatteryTimer.Instance?.PauseTimer();
                Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary).ClearAlarm();
                Teleprinter primaryTP = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Primary);
                
                List<string> cbtLines = new List<string>();
                
                cbtLines.Add("All Counter-Battery assets destroyed. Good Work Operator!");

                primaryTP.ClearAlarm();
                primaryTP.SignalAlarm(Teleprinter.TeleprinterAlarmState.Sucess);
                primaryTP
                    .SubmitLines(
                        Guid.NewGuid().ToString(),
                        cbtLines.Cast<IEnumerable<string>>(),
                        null,
                        false
                    );
            }
        }
    }
}