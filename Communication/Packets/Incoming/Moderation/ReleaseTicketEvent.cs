namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ReleaseTicketEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("mod"))
        {
            return;
        }

        var num = packet.PopInt();
        for (var index = 0; index < num; ++index)
        {
            ModerationManager.ReleaseTicket(Session, packet.PopInt());
        }
    }
}
