namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;

internal class PickTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        _ = packet.PopInt();
        WibboEnvironment.GetGame().GetModerationManager().PickTicket(session, packet.PopInt());
    }
}
