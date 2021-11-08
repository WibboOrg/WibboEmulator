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
                RoomDao.Delete(dbClient, RoomId);
                UserFavoriteDao.Delete(dbClient, RoomId);
                RoomRightDao.Delete(dbClient, RoomId);
                ItemDao.DeleteAllByRoomId(dbClient, RoomId);
                UserDao.UpdateHomeRoom(dbClient, Session.GetHabbo().Id, 0);
                BotDao.UpdateRoomBot(dbClient, RoomId);
                PetDao.UpdateRoomIdByRoomId(dbClient, RoomId);
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