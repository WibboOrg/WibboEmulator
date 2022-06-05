using Wibbo.Communication.Packets.Outgoing.Avatar;
using Wibbo.Communication.Packets.Outgoing.Handshake;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Quests;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
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

            Look = WibboEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_LOOK, 0);

            Session.GetUser().Look = Look;
            Session.GetUser().Gender = Gender.ToLower();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateLookAndGender(dbClient, Session.GetUser().Id, Look, Gender);
            }

            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AvatarLooks", 1);

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

            if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
            {
                return;
            }

            Session.SendPacket(new FigureUpdateComposer(Session.GetUser().Look, Session.GetUser().Gender));
            Session.SendPacket(new UserObjectComposer(Session.GetUser()));
            Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));

            currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
        }
    }
}