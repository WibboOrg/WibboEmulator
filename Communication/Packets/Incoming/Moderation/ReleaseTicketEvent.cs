namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class ReleaseTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var num = Packet.PopInt();
        for (var index = 0; index < num; ++index)
        {
            WibboEnvironment.GetGame().GetModerationManager().ReleaseTicket(session, Packet.PopInt());
        }
    }
}
