namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Utilities;

internal class SubmitNewTicketEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (WibboEnvironment.GetGame().GetModerationManager().UsersHasPendingTicket(session.User.Id))
        {
            return;
        }

        var message = StringCharFilter.Escape(packet.PopString());
        var ticketType = packet.PopInt();
        var reporterId = packet.PopInt();

        _ = packet.PopInt();

        _ = packet.PopInt();
        //chatEntries = packet.PopString();

        if (reporterId == session.User.Id)
        {
            return;
        }

        WibboEnvironment.GetGame().GetModerationManager().SendNewTicket(session, ticketType, reporterId, message);
        ModerationManager.ApplySanction(session, reporterId);
        WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
    }
}
