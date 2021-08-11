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
using Butterfly.Communication.Packets.Outgoing.Navigator.New;
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
using Butterfly.HabboHotel.Users;
using System.Text;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            StringBuilder DeleteParams = new StringBuilder();
            int Amount = Packet.PopInt();
            for (int index = 0; index < Amount; ++index)
            {
                if (index > 0)
                {
                    DeleteParams.Append(" OR ");
                }

                int UserId = Packet.PopInt();
                if (room.UsersWithRights.Contains(UserId))
                {
                    room.UsersWithRights.Remove(UserId);
                }

                DeleteParams.Append("room_id = '" + room.Id + "' AND user_id = '" + UserId + "'");

                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (roomUserByHabbo != null && !roomUserByHabbo.IsBot)
                {
                    roomUserByHabbo.GetClient().SendPacket(new YouAreControllerComposer(0));

                    roomUserByHabbo.RemoveStatus("flatctrl 1");
                    roomUserByHabbo.SetStatus("flatctrl 0", "");
                    roomUserByHabbo.UpdateNeeded = true;
                }
                ServerPacket Response2 = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE);
                Response2.WriteInteger(room.Id);
                Response2.WriteInteger(UserId);
                Session.SendPacket(Response2);

                if (room.UsersWithRights.Count <= 0)
                {
                    ServerPacket Response3 = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST);
                    Response3.WriteInteger(room.RoomData.Id);
                    Response3.WriteInteger(0);
                    Session.SendPacket(Response3);
                }
                else
                {

                    ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST);
                    Response.WriteInteger(room.RoomData.Id);
                    Response.WriteInteger(room.UsersWithRights.Count);
                    foreach (int UserId2 in room.UsersWithRights)
                    {
                        Habbo habboForId = ButterflyEnvironment.GetHabboById(UserId2);
                        Response.WriteInteger(UserId2);
                        Response.WriteString((habboForId == null) ? "Undefined (error)" : habboForId.Username);
                    }
                    Session.SendPacket(Response);
                }
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("DELETE FROM room_rights WHERE " + (DeleteParams).ToString());
            }
        }
    }
}
