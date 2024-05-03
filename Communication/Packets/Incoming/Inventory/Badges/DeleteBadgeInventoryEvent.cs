namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.Badges;
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

        if (BadgeManager.HaveNotAllowed(badgeCode) || !session.User.BadgeComponent.HasBadge(badgeCode))
        {
            session.SendHugeNotification(LanguageManager.TryGetValue("notif.badge.removed.error", session.Language));
            return;
        }

        session.User.BadgeComponent.RemoveBadge(badgeCode);
    }
}
