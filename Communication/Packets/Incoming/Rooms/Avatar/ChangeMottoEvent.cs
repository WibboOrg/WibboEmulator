using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ChangeMottoEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(GameClient Session, ClientPacket Packet)
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

            if (!Session.GetUser().HasPermission("perm_word_filter_override"))
            {
                newMotto = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(newMotto);
            }

            if (Session.GetUser().IgnoreAll)
            {
                return;
            }

            Session.GetUser().Motto = newMotto;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateMotto(dbClient, Session.GetUser().Id, newMotto);
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_MOTTO, 0);

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

                if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
                {
                    return;
                }

                currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
            }

            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Motto", 1);
        }
    }
}