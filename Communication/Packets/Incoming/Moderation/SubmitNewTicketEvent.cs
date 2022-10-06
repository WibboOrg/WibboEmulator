namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;
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

        var Message = StringCharFilter.Escape(packet.PopString());
        var TicketType = packet.PopInt();
        var ReporterId = packet.PopInt();
        var RoomId = packet.PopInt();
        var chatEntriesCount = packet.PopInt();
        //chatEntries = packet.PopString();

        WibboEnvironment.GetGame().GetModerationManager().SendNewTicket(session, TicketType, ReporterId, Message);
        WibboEnvironment.GetGame().GetModerationManager().ApplySanction(session, ReporterId);
        WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
    }
}
