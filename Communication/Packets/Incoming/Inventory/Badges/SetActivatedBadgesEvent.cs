using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SetActivatedBadgesEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null)
                return;
            if (Session.GetUser() == null)
                return;
            if (Session.GetUser().GetBadgeComponent() == null)
                return;

            Session.GetUser().GetBadgeComponent().ResetSlots();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserBadgeDao.UpdateResetSlot(dbClient, Session.GetUser().Id);
            }

            for (int i = 0; i < 5; i++)
            {
                int Slot = Packet.PopInt();
                string Badge = Packet.PopString();

                if (string.IsNullOrEmpty(Badge))
                {
                    continue;
                }

                if (!Session.GetUser().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)
                {
                    continue;
                }

                Session.GetUser().GetBadgeComponent().GetBadge(Badge).Slot = Slot;

                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserBadgeDao.UpdateSlot(dbClient, Session.GetUser().Id, Slot, Badge);
                }
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_BADGE, 0);

            if (!Session.GetUser().InRoom)
                Session.SendPacket(new UserBadgesComposer(Session.GetUser()));
            else 
            {
                Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);

                if(room != null)
                    room.SendPacket(new UserBadgesComposer(Session.GetUser()));
            }
        }
    }
}
