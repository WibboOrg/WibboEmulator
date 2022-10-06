namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class ModerationBanEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_ban"))
        {
            return;
        }

        var UserId = Packet.PopInt();
        var Message = Packet.PopString();
        var Length = Packet.PopInt() * 3600;

        ModerationManager.BanUser(session, UserId, Length, Message);
    }
}