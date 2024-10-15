namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class AddFavouriteRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        var roomId = packet.PopInt();

        var roomData = RoomManager.GenerateRoomData(roomId);
        if (roomData == null || Session.User.FavoriteRooms.Count >= 30 || Session.User.FavoriteRooms.Contains(roomId))
        {
            return;
        }
        else
        {
            Session.SendPacket(new UpdateFavouriteRoomComposer(roomId, true));

            Session.User.FavoriteRooms.Add(roomId);

            using var dbClient = DatabaseManager.Connection;
            UserFavoriteDao.Insert(dbClient, Session.User.Id, roomId);
        }
    }
}
