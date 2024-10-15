namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class SetActivatedBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || Session.User.BadgeComponent == null)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        UserBadgeDao.UpdateResetSlot(dbClient, Session.User.Id);

        Session.User.BadgeComponent.ResetSlots();

        var maxBadgeCount = Session.User.BadgeComponent.BadgeMaxCount;

        for (var i = 0; i < maxBadgeCount; i++)
        {
            var slot = packet.PopInt();
            var badge = packet.PopString();

            if (string.IsNullOrEmpty(badge))
            {
                continue;
            }

            if (!Session.User.BadgeComponent.HasBadge(badge) || slot < 1 || slot > maxBadgeCount)
            {
                continue;
            }

            Session.User.BadgeComponent.GetBadge(badge).Slot = slot;

            UserBadgeDao.UpdateSlot(dbClient, Session.User.Id, slot, badge);
        }

        QuestManager.ProgressUserQuest(Session, QuestType.ProfileBadge, 0);

        if (!Session.User.InRoom)
        {
            Session.SendPacket(new UserBadgesComposer(Session.User));
        }
        else
        {
            if (RoomManager.TryGetRoom(Session.User.RoomId, out var room))
            {
                room.SendPacket(new UserBadgesComposer(Session.User));
            }
        }
    }
}
