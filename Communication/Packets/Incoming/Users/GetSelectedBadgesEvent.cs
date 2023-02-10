namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal sealed class GetSelectedBadgesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();

        var user = WibboEnvironment.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        if (user.BadgeComponent == null)
        {
            return;
        }

        session.SendPacket(new UserBadgesComposer(user));
    }
}
