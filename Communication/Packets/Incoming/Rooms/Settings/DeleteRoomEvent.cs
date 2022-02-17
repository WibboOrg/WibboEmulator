using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeleteRoomEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
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
                BotUserDao.UpdateRoomBot(dbClient, RoomId);
                BotPetDao.UpdateRoomIdByRoomId(dbClient, RoomId);
            }

            if (Session.GetHabbo().UsersRooms.Contains(RoomId))
            {
                Session.GetHabbo().UsersRooms.Remove(RoomId);
            }

            if (Session.GetHabbo().FavoriteRooms != null)
            {
                if (Session.GetHabbo().FavoriteRooms.Contains(RoomId))
                {
                    Session.GetHabbo().FavoriteRooms.Remove(RoomId);
                }
            }
        }
    }
}