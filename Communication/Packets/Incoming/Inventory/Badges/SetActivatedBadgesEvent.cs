namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class SetActivatedBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || session.User.BadgeComponent == null)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        UserBadgeDao.UpdateResetSlot(dbClient, session.User.Id);

        session.User.BadgeComponent.ResetSlots();

        var maxBadgeCount = session.User.BadgeComponent.BadgeMaxCount();

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

        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ProfileBadge, 0);

        if (!session.User.InRoom)
        {
            session.SendPacket(new UserBadgesComposer(session.User));
        }
        else
        {
            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
            {
                room.SendPacket(new UserBadgesComposer(session.User));
            }
        }
    }
}
