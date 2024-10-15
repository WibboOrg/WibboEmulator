namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveRoomScoreEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (session.User.RatedRooms.Contains(room.Id) || room.CheckRights(session, true))
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

        session.User.RatedRooms.Add(room.Id);
        session.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(session.User.RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == session.User.Id)));
    }
}
