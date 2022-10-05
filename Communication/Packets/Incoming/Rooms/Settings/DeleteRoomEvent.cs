namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;

internal class DeleteRoomEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var roomId = Packet.PopInt();

        if (session == null || session.GetUser() == null || session.GetUser().UsersRooms == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!(room.RoomData.OwnerName == session.GetUser().Username))
        {
            return;
        }

        if (session.GetUser().GetInventoryComponent() != null)
        {
            session.GetUser().GetInventoryComponent().AddItemArray(room.GetRoomItemHandler().RemoveAllFurniture(session));
        }

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.Delete(dbClient, roomId);
            UserFavoriteDao.Delete(dbClient, roomId);
            RoomRightDao.Delete(dbClient, roomId);
            ItemDao.DeleteAllByRoomId(dbClient, roomId);
            UserDao.UpdateHomeRoom(dbClient, session.GetUser().Id, 0);
            BotUserDao.UpdateRoomBot(dbClient, roomId);
            BotPetDao.UpdateRoomIdByRoomId(dbClient, roomId);
        }

        if (session.GetUser().UsersRooms.Contains(roomId))
        {
            session.GetUser().UsersRooms.Remove(roomId);
        }

        if (session.GetUser().FavoriteRooms != null)
        {
            if (session.GetUser().FavoriteRooms.Contains(roomId))
            {
                session.GetUser().FavoriteRooms.Remove(roomId);
            }
        }
    }
}