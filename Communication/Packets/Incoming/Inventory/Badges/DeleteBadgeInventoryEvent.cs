namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.GameClients;

internal sealed class DeleteBadgeInventoryEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var badgeCode = packet.PopString();

        session.User.BadgeComponent.RemoveBadge(badgeCode);

        session.SendPacket(new BadgesComposer(session.User.BadgeComponent.BadgeList));
    }
}
