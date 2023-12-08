namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;

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

        if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(badgeCode))
        {
            session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("notif.badge.removed.error", session.Langue));
            return;
        }

        session.User.BadgeComponent.RemoveBadge(badgeCode);
    }
}
