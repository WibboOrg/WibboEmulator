namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class PickTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("mod"))
        {
            return;
        }

        _ = packet.PopInt();
        ModerationManager.PickTicket(Session, packet.PopInt());
    }
}
