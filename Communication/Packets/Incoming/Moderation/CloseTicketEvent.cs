namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;

internal class CloseTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var Result = packet.PopInt();
        _ = packet.PopInt();
        var TicketId = packet.PopInt();

        WibboEnvironment.GetGame().GetModerationManager().CloseTicket(session, packet.PopInt(), Result);
    }
}
