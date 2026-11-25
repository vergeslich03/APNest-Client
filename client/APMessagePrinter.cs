using System.Collections.Concurrent;
using System.Text;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Il2CppSystem.Collections.Generic;

namespace APNestClient;

public class APMessagePrinter
{
    private const string ProgressionColor = "#6A1B9A";
    private const string UsefulColor = "#1F4FBF";
    private const string TrapColor = "#B71C1C";
    private const string FillerColor = "#00695C";
    private const string ActivePlayerColor = "#8D5A00";
    private const string OtherPlayerColor = "#4E342E";
    private const string LocationColor = "#2E7D32";

    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

    private string _slotname;

    public APMessagePrinter(string slotname)
    {
        _slotname = slotname;
    }

    public void Enqueue(LogMessage message)
    {
        if (message is ItemSendLogMessage itemMessage && itemMessage.IsRelatedToActivePlayer)
        {
            _messageQueue.Enqueue(ToRichText(itemMessage));
        }
    }

    public void Print()
    {
        if (!MissionReadyGuard.IsReady()) { return; }

        // the tertiary is created a frame after the mission is ready, keep messages queued until then
        if (TertiaryTeleprinter.Printer == null) { return; }

        List<string> msgs = new List<string>();

        int count = _messageQueue.Count;

        if (count == 0) { return; }

        for (int c = 0; c < count; c++)
        {
            _messageQueue.TryDequeue(out string msg);
            msgs.Add(msg);
        }

        TeleprinterHelper.printTextOnTeleprinter(msgs, TeleprinterType.Archipelago, Teleprinter.TeleprinterAlarmState.None);
    }

    private static string ToRichText(LogMessage message)
    {
        StringBuilder sb = new StringBuilder();
        foreach (MessagePart part in message.Parts)
        {
            string color = ColorOf(part);
            if (color == null)
            {
                sb.Append("<noparse>").Append(part.Text).Append("</noparse>");
            }
            else
            {
                sb.Append("<color=").Append(color).Append("><noparse>").Append(part.Text).Append("</noparse></color>");
            }
        }
        return sb.ToString();
    }

    private static string ColorOf(MessagePart part)
    {
        switch (part)
        {
            case ItemMessagePart item:
                if ((item.Flags & ItemFlags.Trap) != 0) { return TrapColor; }
                if ((item.Flags & ItemFlags.Advancement) != 0) { return ProgressionColor; }
                if ((item.Flags & ItemFlags.NeverExclude) != 0) { return UsefulColor; }
                return FillerColor;
            case PlayerMessagePart player:
                return player.IsActivePlayer ? ActivePlayerColor : OtherPlayerColor;
            case LocationMessagePart:
                return LocationColor;
            default:
                return null;
        }
    }
}
