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

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || session.User.BadgeComponent == null)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        UserBadgeDao.UpdateResetSlot(dbClient, session.User.Id);

        session.User.BadgeComponent.ResetSlots();

        var maxBadgeCount = session.User.BadgeComponent.BadgeMaxCount;

        for (var i = 0; i < maxBadgeCount; i++)
        {
            var slot = packet.PopInt();
            var badge = packet.PopString();

            if (string.IsNullOrEmpty(badge))
            {
                continue;
            }

            if (!session.User.BadgeComponent.HasBadge(badge) || slot < 1 || slot > maxBadgeCount)
            {
                continue;
            }

            session.User.BadgeComponent.GetBadge(badge).Slot = slot;

            UserBadgeDao.UpdateSlot(dbClient, session.User.Id, slot, badge);
        }

        QuestManager.ProgressUserQuest(session, QuestType.ProfileBadge, 0);

        if (!session.User.InRoom)
        {
            session.SendPacket(new UserBadgesComposer(session.User));
        }
        else
        {
            if (RoomManager.TryGetRoom(session.User.RoomId, out var room))
            {
                room.SendPacket(new UserBadgesComposer(session.User));
            }
        }
    }
}
