using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DeleteRoomEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            if (Session == null || Session.GetUser() == null || Session.GetUser().UsersRooms == null)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null || !(room.RoomData.OwnerName == Session.GetUser().Username))
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
                RoomDao.Delete(dbClient, RoomId);
                UserFavoriteDao.Delete(dbClient, RoomId);
                RoomRightDao.Delete(dbClient, RoomId);
                ItemDao.DeleteAllByRoomId(dbClient, RoomId);
                UserDao.UpdateHomeRoom(dbClient, Session.GetUser().Id, 0);
                BotUserDao.UpdateRoomBot(dbClient, RoomId);
                BotPetDao.UpdateRoomIdByRoomId(dbClient, RoomId);
            }

            if (Session.GetUser().UsersRooms.Contains(RoomId))
            {
                Session.GetUser().UsersRooms.Remove(RoomId);
            }

            if (Session.GetUser().FavoriteRooms != null)
            {
                if (Session.GetUser().FavoriteRooms.Contains(RoomId))
                {
                    Session.GetUser().FavoriteRooms.Remove(RoomId);
                }
            }
        }
    }
}