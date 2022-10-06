namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class ModerationBanEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_ban"))
        {
            return;
        }

        var UserId = packet.PopInt();
        var Message = packet.PopString();
        var Length = packet.PopInt() * 3600;

        ModerationManager.BanUser(session, UserId, Length, Message);
    }
}