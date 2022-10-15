namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.GameClients;

internal class GetBadgesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        session.SendPacket(new BadgesComposer(session.User.BadgeComponent.BadgeList));
    }
}