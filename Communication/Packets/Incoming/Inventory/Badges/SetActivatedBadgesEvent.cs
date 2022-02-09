using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SetActivatedBadgesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null)
                return;
            if (Session.GetHabbo() == null)
                return;
            if (Session.GetHabbo().GetBadgeComponent() == null)
                return;

            Session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserBadgeDao.UpdateResetSlot(dbClient, Session.GetHabbo().Id);
            }

            for (int i = 0; i < 5; i++)
            {
                int Slot = Packet.PopInt();
                string Badge = Packet.PopString();

                if (string.IsNullOrEmpty(Badge))
                {
                    continue;
                }

                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)
                {
                    continue;
                }

                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserBadgeDao.UpdateSlot(dbClient, Session.GetHabbo().Id, Slot, Badge);
                }
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_BADGE, 0);

            if (!Session.GetHabbo().InRoom)
                Session.SendPacket(new HabboUserBadgesComposer(Session.GetHabbo()));
            else 
            {
                Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                if(room != null)
                    room.SendPacket(new HabboUserBadgesComposer(Session.GetHabbo()));
            }
        }
    }
}
