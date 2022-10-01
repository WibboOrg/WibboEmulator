using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByName(Session.GetUser().Username);
            if (roomUser == null)
            {
                return;
            }

            string newUsername = Packet.PopString();

            if (!Session.GetUser().CanChangeName && Session.GetUser().Rank == 1)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.1", Session.Langue));
                return;
            }

            if (newUsername == Session.GetUser().Username)
            {
                Session.SendPacket(new UpdateUsernameComposer(Session.GetUser().Username));
                return;
            }

            if (this.NameAvailable(newUsername) != 1)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.2", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateOwner(dbClient, newUsername, Session.GetUser().Username);

                UserDao.UpdateName(dbClient, Session.GetUser().Id, newUsername);

                LogFlagmeDao.Insert(dbClient, Session.GetUser().Id, Session.GetUser().Username, newUsername);
            }

            WibboEnvironment.GetGame().GetClientManager().UpdateClientUsername(Session.ConnectionID, Session.GetUser().Username, newUsername);
            room.GetRoomUserManager().UpdateClientUsername(roomUser, Session.GetUser().Username, newUsername);
            Session.GetUser().Username = newUsername;
            Session.GetUser().CanChangeName = false;

            Session.SendPacket(new UpdateUsernameComposer(newUsername));
            Session.SendPacket(new UserObjectComposer(Session.GetUser()));

            foreach (int roomId in Session.GetUser().UsersRooms)
            {
                if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room roomOwner))
                    continue;

                roomOwner.RoomData.OwnerName = newUsername;

                WibboEnvironment.GetGame().GetRoomManager().RoomDataRemove(roomId);
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

            if (!WibboEnvironment.IsValidAlphaNumeric(Username))
            {
                return -1;
            }

            return WibboEnvironment.UsernameExists(Username) ? 0 : 1;
        }
    }
}
