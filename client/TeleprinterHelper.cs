using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace APNestClient;

public class TeleprinterHelper
{
    public static void printTextOnPrimary(List<string> text, Teleprinter.TeleprinterAlarmState alarmState)
    {
        Teleprinter primary = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Primary);
        
        primary.SignalAlarm(alarmState);
        primary
            .SubmitLines(
                Guid.NewGuid().ToString(),
                text.Cast<IEnumerable<string>>(),
                null,
                false
            );
    }

    public static void printTextOnSecondary(List<string> text, Teleprinter.TeleprinterAlarmState alarmState)
    {
        Teleprinter secondary =  Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary);
        
        secondary.SignalAlarm(alarmState);
        secondary
            .SubmitLines(
                Guid.NewGuid().ToString(),
                text.Cast<IEnumerable<string>>(),
                null,
                false
            );
    }
}