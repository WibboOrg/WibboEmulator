using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;
using Butterfly.Utilities;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ChangeMottoEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string newMotto = StringCharFilter.Escape(Packet.PopString());
            if (newMotto == Session.GetUser().Motto)
            {
                return;
            }

            if (newMotto.Length > 38)
            {
                newMotto = newMotto.Substring(0, 38);
            }

            if (Session.Antipub(newMotto, "<MOTTO>"))
            {
                return;
            }

            if (!Session.GetUser().HasFuse("word_filter_override"))
            {
                newMotto = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(newMotto);
            }

            if (Session.GetUser().IgnoreAll)
            {
                return;
            }

            Session.GetUser().Motto = newMotto;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateMotto(dbClient, Session.GetUser().Id, newMotto);
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_MOTTO, 0);

            if (Session.GetUser().InRoom)
            {
                Room currentRoom = Session.GetUser().CurrentRoom;
                if (currentRoom == null)
                {
                    return;
                }

                RoomUser roomUserByUserId = currentRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                if (roomUserByUserId == null)
                {
                    return;
                }

                if (roomUserByUserId.transformation || roomUserByUserId.IsSpectator)
                {
                    return;
                }

                currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
            }

            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Motto", 1);
        }
    }
}