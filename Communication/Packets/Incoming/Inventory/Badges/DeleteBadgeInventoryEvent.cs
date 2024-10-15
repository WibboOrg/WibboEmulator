namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.GameClients;

internal sealed class DeleteBadgeInventoryEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        var badgeCode = packet.PopString();

        if (!BadgeManager.CanDeleteBadge(badgeCode) || !Session.User.BadgeComponent.HasBadge(badgeCode))
        {
            Session.SendHugeNotification(LanguageManager.TryGetValue("notif.badge.removed.error", Session.Language));
            return;
        }

        Session.User.BadgeComponent.RemoveBadge(badgeCode);
    }
}
