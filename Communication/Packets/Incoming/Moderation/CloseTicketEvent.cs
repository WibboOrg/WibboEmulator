namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class CloseTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.HasPermission("mod"))
        {
            return;
        }

        var result = packet.PopInt();
        _ = packet.PopInt();
        var ticketId = packet.PopInt();

        ModerationManager.CloseTicket(Session, ticketId, result);
    }
}
