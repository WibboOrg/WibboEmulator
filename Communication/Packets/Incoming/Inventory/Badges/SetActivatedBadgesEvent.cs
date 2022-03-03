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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserBadgeDao.UpdateSlot(dbClient, Session.GetUser().Id, Slot, Badge);
                }
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_BADGE, 0);

            if (!Session.GetUser().InRoom)
                Session.SendPacket(new UserBadgesComposer(Session.GetUser()));
            else 
            {
                Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);

                if(room != null)
                    room.SendPacket(new UserBadgesComposer(Session.GetUser()));
            }
        }
    }
}
