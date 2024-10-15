namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveRoomScoreEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (Session.User.RatedRooms.Contains(room.Id) || room.CheckRights(Session, true))
        {
            return;
        }

        var score = packet.PopInt();
        switch (score)
        {
            case -1:
                room.RoomData.Score--;
                break;
            case 0:
                return;
            case 1:
                room.RoomData.Score++;
                break;
            default:
                return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateScore(dbClient, room.Id, room.RoomData.Score);
        }

        Session.User.RatedRooms.Add(room.Id);
        Session.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(Session.User.RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == Session.User.Id)));
    }
}
