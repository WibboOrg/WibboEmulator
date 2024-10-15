namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DeleteRoomEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        if (Session == null || Session.User == null || Session.User.UsersRooms == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!(room.RoomData.OwnerName == Session.User.Username))
        {
            return;
        }

        if (room.RoomData.Group != null)
        {
            return;
            // TODO: a notification like "you must first delete the group of this room" 
        }

        Session.User.InventoryComponent?.AddItemArray(room.RoomItemHandling.RemoveAllFurnitureToInventory(Session));

        RoomManager.UnloadRoom(room);

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.Delete(dbClient, roomId);
            UserFavoriteDao.Delete(dbClient, roomId);
            RoomRightDao.Delete(dbClient, roomId);
            RoomBanDao.Delete(dbClient, roomId);
            ItemDao.DeleteAllByRoomId(dbClient, roomId);
            UserDao.UpdateHomeRoom(dbClient, Session.User.Id, 0);
            BotUserDao.UpdateRoomBot(dbClient, roomId);
            BotPetDao.UpdateRoomIdByRoomId(dbClient, roomId);
        }

        if (Session.User.UsersRooms.Contains(roomId))
        {
            _ = Session.User.UsersRooms.Remove(roomId);
        }

        if (Session.User.FavoriteRooms != null)
        {
            if (Session.User.FavoriteRooms.Contains(roomId))
            {
                _ = Session.User.FavoriteRooms.Remove(roomId);
            }
        }
    }
}
