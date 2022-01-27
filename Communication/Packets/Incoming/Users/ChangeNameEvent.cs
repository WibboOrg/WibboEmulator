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
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null || Session == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByName(Session.GetHabbo().Username);
            if (roomUser == null)
            {
                return;
            }

            string newUsername = Packet.PopString();

            if (!Session.GetHabbo().CanChangeName && Session.GetHabbo().Rank == 1)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.1", Session.Langue));
                return;
            }

            if (newUsername == Session.GetHabbo().Username)
            {
                Session.SendPacket(new UpdateUsernameComposer(Session.GetHabbo().Username));
                return;
            }

            if (this.NameAvailable(newUsername) != 1)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.2", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateOwner(dbClient, newUsername, Session.GetHabbo().Username);

                UserDao.UpdateName(dbClient, Session.GetHabbo().Id, newUsername);

                LogFlagmeDao.Insert(dbClient, Session.GetHabbo().Id, Session.GetHabbo().Username, newUsername);
            }

            ButterflyEnvironment.GetGame().GetClientManager().UpdateClientUsername(Session.ConnectionID, Session.GetHabbo().Username, newUsername);
            room.GetRoomUserManager().UpdateClientUsername(roomUser, Session.GetHabbo().Username, newUsername);
            Session.GetHabbo().Username = newUsername;
            Session.GetHabbo().CanChangeName = false;

            Session.SendPacket(new UpdateUsernameComposer(newUsername));
            Session.SendPacket(new UserObjectComposer(Session.GetHabbo()));

            foreach (int RoomId in Session.GetHabbo().UsersRooms)
            {
                Room roomowner = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
                if (roomowner != null)
                {
                    roomowner.RoomData.OwnerName = newUsername;
                }

                ButterflyEnvironment.GetGame().GetRoomManager().RoomDataRemove(RoomId);
            }

            room.SendPacket(new UserNameChangeComposer(newUsername, roomUser.VirtualId));

            if (Session.GetHabbo().Id == room.RoomData.OwnerId)
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
