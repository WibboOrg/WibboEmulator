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
        if (session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
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
            var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(num);
            if (roomUserByUserId != null)
            {
                if (!roomUserByUserId.IsBot)
                {
                    roomUserByUserId.GetClient().SendPacket(new YouAreControllerComposer(0));

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
