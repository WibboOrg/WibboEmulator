namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;

internal sealed class CloseTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.HasPermission("mod"))
        {
            return;
        }

        var result = packet.PopInt();
        _ = packet.PopInt();
        var ticketId = packet.PopInt();

        WibboEnvironment.GetGame().GetModerationManager().CloseTicket(session, ticketId, result);
    }
}
