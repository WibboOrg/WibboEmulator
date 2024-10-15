namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class RemoveFavouriteRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        var roomId = packet.PopInt();

        if (!Session.User.FavoriteRooms.Contains(roomId))
        {
            return;
        }

        _ = Session.User.FavoriteRooms.Remove(roomId);

        Session.SendPacket(new UpdateFavouriteRoomComposer(roomId, false));

        using var dbClient = DatabaseManager.Connection;
        UserFavoriteDao.Delete(dbClient, Session.User.Id, roomId);
    }
}
