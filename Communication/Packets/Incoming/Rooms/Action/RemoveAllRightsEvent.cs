namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveAllRightsEvent : IPacketEvent
{
    public double Delay => 1000;

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

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        if (room.UsersWithRights.Count == 0)
        {
            return;
        }

        foreach (var num in room.UsersWithRights)
        {
            var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(num);
            if (roomUserByUserId != null)
            {
                if (!roomUserByUserId.IsBot)
                {
                    roomUserByUserId.Client.SendPacket(new YouAreControllerComposer(0));

                    roomUserByUserId.UpdateNeeded = true;
                }
            }

            Session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, num));
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomRightDao.Delete(dbClient, room.Id);
        }

        room.UsersWithRights.Clear();

        Session.SendPacket(new RoomRightsListComposer(room));
    }
}
