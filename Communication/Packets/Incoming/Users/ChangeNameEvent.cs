using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE rooms SET owner = @newname WHERE owner = @oldname");
                queryreactor.AddParameter("newname", NewUsername);
                queryreactor.AddParameter("oldname", Session.GetHabbo().Username);
                queryreactor.RunQuery();

                queryreactor.SetQuery("UPDATE users SET username = @newname WHERE id = @userid");
                queryreactor.AddParameter("newname", NewUsername);
                queryreactor.AddParameter("userid", Session.GetHabbo().Id);
                queryreactor.RunQuery();

                queryreactor.SetQuery("INSERT INTO `logs_flagme` (`user_id`, `oldusername`, `newusername`, `time`) VALUES (@userid, @oldusername, @newusername, '" + ButterflyEnvironment.GetUnixTimestamp() + "');");
                queryreactor.AddParameter("userid", Session.GetHabbo().Id);
                queryreactor.AddParameter("oldusername", Session.GetHabbo().Username);
                queryreactor.AddParameter("newusername", NewUsername);
                queryreactor.RunQuery();
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
