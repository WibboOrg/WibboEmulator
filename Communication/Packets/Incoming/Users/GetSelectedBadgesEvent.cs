namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal sealed class GetSelectedBadgesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var userId = packet.PopInt();

        var user = UserManager.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        if (user.BadgeComponent == null)
        {
            return;
        }

        Session.SendPacket(new UserBadgesComposer(user));
    }
}
