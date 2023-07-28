namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class DeleteRoomEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        if (session == null || session.User == null || session.User.UsersRooms == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!(room.RoomData.OwnerName == session.User.Username))
        {
            return;
        }

        session.User.InventoryComponent?.AddItemArray(room.RoomItemHandling.RemoveAllFurnitureToInventory(session));

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.Delete(dbClient, roomId);
            UserFavoriteDao.Delete(dbClient, roomId);
            RoomRightDao.Delete(dbClient, roomId);
            ItemDao.DeleteAllByRoomId(dbClient, roomId);
            UserDao.UpdateHomeRoom(dbClient, session.User.Id, 0);
            BotUserDao.UpdateRoomBot(dbClient, roomId);
            BotPetDao.UpdateRoomIdByRoomId(dbClient, roomId);
        }

        if (session.User.UsersRooms.Contains(roomId))
        {
            _ = session.User.UsersRooms.Remove(roomId);
        }

        if (session.User.FavoriteRooms != null)
        {
            if (session.User.FavoriteRooms.Contains(roomId))
            {
                _ = session.User.FavoriteRooms.Remove(roomId);
            }
        }
    }
}
