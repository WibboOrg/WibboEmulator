namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;

internal sealed class PickTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("mod"))
        {
            return;
        }

        _ = packet.PopInt();
        WibboEnvironment.GetGame().GetModerationManager().PickTicket(session, packet.PopInt());
    }
}
