namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class CloseTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var Result = Packet.PopInt();
        Packet.PopInt();
        var TicketId = Packet.PopInt();

        WibboEnvironment.GetGame().GetModerationManager().CloseTicket(session, Packet.PopInt(), Result);
    }
}
