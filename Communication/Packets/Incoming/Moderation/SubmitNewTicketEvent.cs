namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;
using WibboEmulator.Utilities;

internal class SubmitNewTicketEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (WibboEnvironment.GetGame().GetModerationManager().UsersHasPendingTicket(session.GetUser().Id))
        {
            return;
        }

        var message = StringCharFilter.Escape(packet.PopString());
        var ticketType = packet.PopInt();
        var reporterId = packet.PopInt();
        var roomId = packet.PopInt();
        var chatEntriesCount = packet.PopInt();
        //chatEntries = packet.PopString();

        WibboEnvironment.GetGame().GetModerationManager().SendNewTicket(session, ticketType, reporterId, message);
        ModerationManager.ApplySanction(session, reporterId);
        WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
    }
}
