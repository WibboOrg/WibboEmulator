namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class RemoveAllRightsEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
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

            session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, num));
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomRightDao.Delete(dbClient, room.Id);
        }

        room.UsersWithRights.Clear();

        session.SendPacket(new RoomRightsListComposer(room));
    }
}
