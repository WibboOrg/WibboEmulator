namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;

internal sealed class UnbanUserFromRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        var instance = Session.User.Room;
        if (instance == null || !instance.CheckRights(Session, true))
        {
            return;
        }

        var userId = packet.PopInt();
        var roomId = packet.PopInt();

        if (!instance.HasBanExpired(userId))
        {
            using (var dbClient = DatabaseManager.Connection)
            {
                RoomBanDao.Delete(dbClient, instance.Id, Session.User.Id);
            }

            instance.RemoveBan(userId);

            Session.SendPacket(new UnbanUserFromRoomComposer(roomId, userId));
        }
    }
}
