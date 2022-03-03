using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateFigureDataEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            string Gender = Packet.PopString().ToUpper();
            string Look = Packet.PopString();
            if (Gender != "M" && Gender != "F")
            {
                return;
            }

            Look = ButterflyEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_LOOK, 0);

            Session.GetUser().Look = Look;
            Session.GetUser().Gender = Gender.ToLower();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateLookAndGender(dbClient, Session.GetUser().Id, Look, Gender);
            }

            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AvatarLooks", 1);

            if (!Session.GetUser().InRoom)
            {
                return;
            }

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

            Session.SendPacket(new UserObjectComposer(Session.GetUser()));
            Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));

            currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
        }
    }
}