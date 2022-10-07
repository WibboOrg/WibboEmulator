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

        if (session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().GetBadgeComponent() == null)
        {
            return;
        }

        session.GetUser().GetBadgeComponent().ResetSlots();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserBadgeDao.UpdateResetSlot(dbClient, session.GetUser().Id);
        }

        for (var i = 0; i < 5; i++)
        {
            var slot = packet.PopInt();
            var badge = packet.PopString();

            if (string.IsNullOrEmpty(badge))
            {
                continue;
            }

            if (!session.GetUser().GetBadgeComponent().HasBadge(badge) || slot < 1 || slot > 5)
            {
                continue;
            }

            session.GetUser().GetBadgeComponent().GetBadge(badge).Slot = slot;

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserBadgeDao.UpdateSlot(dbClient, session.GetUser().Id, slot, badge);
        }

        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.PROFILE_BADGE, 0);

        if (!session.GetUser().InRoom)
        {
            session.SendPacket(new UserBadgesComposer(session.GetUser()));
        }
        else
        {
            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
            {
                room.SendPacket(new UserBadgesComposer(session.GetUser()));
            }
        }
    }
}
