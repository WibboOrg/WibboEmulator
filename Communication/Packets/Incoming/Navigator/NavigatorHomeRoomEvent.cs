namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class NavigatorHomeRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
        if (roomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != session.User.Username.ToLower()))
        {
            return;
        }

        session.User.HomeRoom = roomId;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateHomeRoom(dbClient, session.User.Id, roomId);
        }

        session.SendPacket(new NavigatorHomeRoomComposer(roomId, 0));
    }
}
