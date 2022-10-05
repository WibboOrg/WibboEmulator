namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal class SubmitNewTicketEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (WibboEnvironment.GetGame().GetModerationManager().UsersHasPendingTicket(session.GetUser().Id))
        {
            return;
        }

        var Message = StringCharFilter.Escape(Packet.PopString());
        var TicketType = Packet.PopInt();
        var ReporterId = Packet.PopInt();
        var RoomId = Packet.PopInt();
        var chatEntriesCount = Packet.PopInt();
        //chatEntries = Packet.PopString();

        WibboEnvironment.GetGame().GetModerationManager().SendNewTicket(session, TicketType, ReporterId, Message);
        WibboEnvironment.GetGame().GetModerationManager().ApplySanction(session, ReporterId);
        WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
    }
}
