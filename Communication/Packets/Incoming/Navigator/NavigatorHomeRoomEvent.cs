namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class NavigatorHomeRoomEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var RoomId = packet.PopInt();
        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
        if (RoomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != session.GetUser().Username.ToLower()))
        {
            return;
        }

        session.GetUser().HomeRoom = RoomId;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateHomeRoom(dbClient, session.GetUser().Id, RoomId);
        }

        session.SendPacket(new NavigatorHomeRoomComposer(RoomId, 0));
    }
}
