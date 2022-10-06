namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal class GetSelectedBadgesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var UserId = Packet.PopInt();

        var User = WibboEnvironment.GetUserById(UserId);
        if (User == null)
        {
            return;
        }

        if (User.GetBadgeComponent() == null)
        {
            return;
        }

        session.SendPacket(new UserBadgesComposer(User));
    }
}