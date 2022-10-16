namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class SetActivatedBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null)
        {
            return;
        }

        if (session.User == null)
        {
            return;
        }

        if (session.User.BadgeComponent == null)
        {
            return;
        }

        session.User.BadgeComponent.ResetSlots();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserBadgeDao.UpdateResetSlot(dbClient, session.User.Id);
        }

        for (var i = 0; i < 5; i++)
        {
            var slot = packet.PopInt();
            var badge = packet.PopString();

            if (string.IsNullOrEmpty(badge))
            {
                continue;
            }

            if (!session.User.BadgeComponent.HasBadge(badge) || slot < 1 || slot > 5)
            {
                continue;
            }

            session.User.BadgeComponent.GetBadge(badge).Slot = slot;

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
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
