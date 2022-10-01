using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DeleteRoomEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int roomId = Packet.PopInt();

            if (Session == null || Session.GetUser() == null || Session.GetUser().UsersRooms == null)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room room))
                return;

            if (!(room.RoomData.OwnerName == Session.GetUser().Username))
            {
                return;
            }

            if (Session.GetUser().GetInventoryComponent() != null)
            {
                Session.GetUser().GetInventoryComponent().AddItemArray(room.GetRoomItemHandler().RemoveAllFurniture(Session));
            }

            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.Delete(dbClient, roomId);
                UserFavoriteDao.Delete(dbClient, roomId);
                RoomRightDao.Delete(dbClient, roomId);
                ItemDao.DeleteAllByRoomId(dbClient, roomId);
                UserDao.UpdateHomeRoom(dbClient, Session.GetUser().Id, 0);
                BotUserDao.UpdateRoomBot(dbClient, roomId);
                BotPetDao.UpdateRoomIdByRoomId(dbClient, roomId);
            }

            if (Session.GetUser().UsersRooms.Contains(roomId))
            {
                Session.GetUser().UsersRooms.Remove(roomId);
            }

            if (Session.GetUser().FavoriteRooms != null)
            {
                if (Session.GetUser().FavoriteRooms.Contains(roomId))
                {
                    Session.GetUser().FavoriteRooms.Remove(roomId);
                }
            }
        }
    }
}