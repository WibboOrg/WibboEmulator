namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal sealed class RemoveRightsEvent : IPacketEvent
{
    public double Delay => 250;

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

        var userIds = new List<int>();
        var amount = packet.PopInt();
        for (var index = 0; index < amount; index++)
        {
            var userId = packet.PopInt();
            if (room.UsersWithRights.Contains(userId))
            {
                _ = room.UsersWithRights.Remove(userId);
            }

            if (!userIds.Contains(userId))
            {
                userIds.Add(userId);
            }

            var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(userId);
            if (roomUserByUserId != null && !roomUserByUserId.IsBot)
            {
                roomUserByUserId.Client?.SendPacket(new YouAreControllerComposer(0));

                roomUserByUserId.RemoveStatus("flatctrl");
                roomUserByUserId.SetStatus("flatctrl", "0");
                roomUserByUserId.UpdateNeeded = true;
            }

            session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, userId));

            if (room.UsersWithRights.Count <= 0)
            {
                session.SendPacket(new RoomRightsListComposer(room));
            }
            else
            {
                _ = room.UsersWithRights.Contains(userId);
                session.SendPacket(new RoomRightsListComposer(room));
            }
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        RoomRightDao.DeleteList(dbClient, room.Id, userIds);
    }
}
