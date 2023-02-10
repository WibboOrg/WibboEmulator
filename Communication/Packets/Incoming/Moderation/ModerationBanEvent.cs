namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModerationBanEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("ban"))
        {
            return;
        }

        var userId = packet.PopInt();
        var message = packet.PopString();
        var length = packet.PopInt() * 3600;

        ModerationManager.BanUser(session, userId, length, message);
    }
}
