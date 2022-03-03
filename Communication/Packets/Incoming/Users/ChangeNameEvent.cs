using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ChangeNameEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null || Session == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByName(Session.GetUser().Username);
            if (roomUser == null)
            {
                return;
            }

            string newUsername = Packet.PopString();

            if (!Session.GetUser().CanChangeName && Session.GetUser().Rank == 1)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.1", Session.Langue));
                return;
            }

            if (newUsername == Session.GetUser().Username)
            {
                Session.SendPacket(new UpdateUsernameComposer(Session.GetUser().Username));
                return;
            }

            if (this.NameAvailable(newUsername) != 1)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.2", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateOwner(dbClient, newUsername, Session.GetUser().Username);

                UserDao.UpdateName(dbClient, Session.GetUser().Id, newUsername);

                LogFlagmeDao.Insert(dbClient, Session.GetUser().Id, Session.GetUser().Username, newUsername);
            }

            ButterflyEnvironment.GetGame().GetClientManager().UpdateClientUsername(Session.ConnectionID, Session.GetUser().Username, newUsername);
            room.GetRoomUserManager().UpdateClientUsername(roomUser, Session.GetUser().Username, newUsername);
            Session.GetUser().Username = newUsername;
            Session.GetUser().CanChangeName = false;

            Session.SendPacket(new UpdateUsernameComposer(newUsername));
            Session.SendPacket(new UserObjectComposer(Session.GetUser()));

            foreach (int RoomId in Session.GetUser().UsersRooms)
            {
                Room roomowner = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
                if (roomowner != null)
                {
                    roomowner.RoomData.OwnerName = newUsername;
                }

                ButterflyEnvironment.GetGame().GetRoomManager().RoomDataRemove(RoomId);
            }

            room.SendPacket(new UserNameChangeComposer(newUsername, roomUser.VirtualId));

            if (Session.GetUser().Id == room.RoomData.OwnerId)
            {
                room.RoomData.OwnerName = newUsername;
                room.SendPacket(new RoomInfoUpdatedComposer(room.Id));
            }
        }

        private int NameAvailable(string Username)
        {
            Username = Username.ToLower();

            if (Username.Length > 15)
            {
                return -2;
            }

            if (Username.Length < 3)
            {
                return -2;
            }

            if (!ButterflyEnvironment.IsValidAlphaNumeric(Username))
            {
                return -1;
            }

            return ButterflyEnvironment.UsernameExists(Username) ? 0 : 1;
        }
    }
}
