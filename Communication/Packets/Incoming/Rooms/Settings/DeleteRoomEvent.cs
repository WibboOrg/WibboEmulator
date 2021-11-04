using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeleteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().UsersRooms == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null || !(room.RoomData.OwnerName == Session.GetHabbo().Username))
            {
                return;
            }

            if (Session.GetHabbo().GetInventoryComponent() != null)
            {
                Session.GetHabbo().GetInventoryComponent().AddItemArray(room.GetRoomItemHandler().RemoveAllFurniture(Session));
            }

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM rooms WHERE id = '" + RoomId + "'");
                dbClient.RunQuery("DELETE FROM user_favorites WHERE room_id = '" + RoomId + "'");
                dbClient.RunQuery("DELETE FROM room_rights WHERE room_id = '" + RoomId + "'");
                dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '" + RoomId + "'");
                dbClient.RunQuery("UPDATE users SET home_room = '0' WHERE id = '" + Session.GetHabbo().Id + "'");
                BotDao.UpdateRoomBot(dbClient, RoomId);
                dbClient.RunQuery("UPDATE pets SET room_id = '0' WHERE room_id = '" + RoomId + "'");
            }

            RoomData removedRoom = (from p in Session.GetHabbo().UsersRooms where p.Id == RoomId select p).SingleOrDefault();
            if (removedRoom != null)
            {
                Session.GetHabbo().UsersRooms.Remove(removedRoom);
            }

            if (Session.GetHabbo().FavoriteRooms != null)
            {
                RoomData removedRoomFavo = (from p in Session.GetHabbo().FavoriteRooms where p.Id == RoomId select p).FirstOrDefault();

                if (removedRoomFavo != null)
                {
                    Session.GetHabbo().FavoriteRooms.Remove(removedRoomFavo);
                }
            }
        }
    }
}