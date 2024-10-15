namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class SubmitNewTicketEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (ModerationManager.UsersHasPendingTicket(Session.User.Id))
        {
            return;
        }

        var message = packet.PopString();
        var ticketType = packet.PopInt();
        var reporterId = packet.PopInt();

        _ = packet.PopInt();

        _ = packet.PopInt();
        //chatEntries = packet.PopString();

        if (reporterId == Session.User.Id)
        {
            return;
        }

        ModerationManager.SendNewTicket(Session, ticketType, reporterId, message);
        ModerationManager.ApplySanction(Session, reporterId);
        GameClientManager.SendMessageStaff(RoomNotificationComposer.SendBubble("mention", "Un nouveau ticket vient d'arriver sur le support"));
    }
}
