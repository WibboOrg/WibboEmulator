namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;

internal sealed class GetModeratorUserInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("mod"))
        {
            return;
        }

        var userId = packet.PopInt();

        var user = WibboEnvironment.GetUserById(userId);

        if (user == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", session.Langue));
            return;
        }

        session.SendPacket(new ModeratorUserInfoComposer(user));
    }
}
