namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;

internal class AddFavouriteRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var roomId = Packet.PopInt();

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
        if (roomData == null || session.GetUser().FavoriteRooms.Count >= 30 || session.GetUser().FavoriteRooms.Contains(roomId))
        {
            return;
        }
        else
        {
            session.SendPacket(new UpdateFavouriteRoomComposer(roomId, true));

            session.GetUser().FavoriteRooms.Add(roomId);

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserFavoriteDao.Insert(dbClient, session.GetUser().Id, roomId);
        }
    }
}