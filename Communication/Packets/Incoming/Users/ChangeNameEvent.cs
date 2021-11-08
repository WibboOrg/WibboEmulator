using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ChangeNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null || Session == null)
            {
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByName(Session.GetHabbo().Username);
            if (User == null)
            {
                return;
            }

            string NewUsername = Packet.PopString();
            if (!Session.GetHabbo().CanChangeName && Session.GetHabbo().Rank == 1)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.1", Session.Langue));
                return;
            }

            if (NewUsername == Session.GetHabbo().Username)
            {
                Session.SendPacket(new UpdateUsernameComposer(Session.GetHabbo().Username));
                return;
            }

            if (this.NameAvailable(NewUsername) != 1)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.2", Session.Langue));
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateOwner(dbClient, NewUsername, Session.GetHabbo().Username);

                UserDao.UpdateName(dbClient, Session.GetHabbo().Id, NewUsername);

                LogFlagmeDao.Insert(dbClient, Session.GetHabbo().Id, Session.GetHabbo().Username, NewUsername);
            }

            ButterflyEnvironment.GetGame().GetClientManager().UpdateClientUsername(Session.ConnectionID, Session.GetHabbo().Username, NewUsername);
            Room.GetRoomUserManager().UpdateClientUsername(User, Session.GetHabbo().Username, NewUsername);
            Session.GetHabbo().Username = NewUsername;
            Session.GetHabbo().CanChangeName = false;

            Session.SendPacket(new UpdateUsernameComposer(NewUsername));
            Session.SendPacket(new UserObjectComposer(Session.GetHabbo()));

            Session.GetHabbo().UpdateRooms();
            foreach (RoomData roomData in Session.GetHabbo().UsersRooms)
            {
                roomData.OwnerName = NewUsername;

                Room roomowner = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(roomData.Id);
                if (roomowner != null)
                {
                    roomowner.RoomData.OwnerName = NewUsername;
                }

                ButterflyEnvironment.GetGame().GetRoomManager().RoomDataRemove(roomData.Id);
            }

            Room.SendPacket(new UserNameChangeMessageComposer(NewUsername, User.VirtualId));

            if (Session.GetHabbo().Id != Room.RoomData.OwnerId)
            {
                return;
            }

            Room.SendPacket(new RoomInfoUpdatedMessageComposer(Room.Id));
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
