namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModerationMuteEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("no_kick"))
        {
            return;
        }

        var userId = packet.PopInt();
        var messageText = packet.PopString();

        ModerationManager.KickUser(session, userId, messageText, false);
    }
}
