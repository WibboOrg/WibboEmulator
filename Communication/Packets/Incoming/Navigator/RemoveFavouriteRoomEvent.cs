namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;

internal class RemoveFavouriteRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var roomId = Packet.PopInt();

        var roomdata = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
        if (roomdata == null)
        {
            return;
        }

        session.GetUser().FavoriteRooms.Remove(roomId);

        session.SendPacket(new UpdateFavouriteRoomComposer(roomId, false));

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        UserFavoriteDao.Delete(dbClient, session.GetUser().Id, roomId);
    }
}
