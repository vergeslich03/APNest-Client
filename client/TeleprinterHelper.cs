using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace APNestClient;

public enum TeleprinterType
{
    Primary,
    Secondary,
    Archipelago,
}

public static class TeleprinterHelper
{
    public static void printTextOnTeleprinter(
        List<string> text,
        TeleprinterType teleprinterType,
        Il2Cpp.Teleprinter.TeleprinterAlarmState alarmState
    )
    {
        Teleprinter teleprinter = null;
        
        switch (teleprinterType)
        {
            case TeleprinterType.Primary:
                teleprinter =  Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Primary);
                break;
            case TeleprinterType.Secondary:
                teleprinter =   Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary);
                break;
            case TeleprinterType.Archipelago:
                teleprinter = TertiaryTeleprinter.Printer;
                break;
        }
        
        teleprinter.SignalAlarm(alarmState);
        teleprinter.SubmitLines(
            Guid.NewGuid().ToString(),
            text.Cast<IEnumerable<string>>(),
            null,
            false
        );
    }
}