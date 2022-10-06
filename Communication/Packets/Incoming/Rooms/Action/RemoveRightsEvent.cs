namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class RemoveRightsEvent : IPacketEvent
{
    public double Delay => 250;

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

        var userIds = new List<int>();
        var Amount = packet.PopInt();
        for (var index = 0; index < Amount; ++index)
        {
            var UserId = packet.PopInt();
            if (room.UsersWithRights.Contains(UserId))
            {
                _ = room.UsersWithRights.Remove(UserId);
            }

            if (!userIds.Contains(UserId))
            {
                userIds.Add(UserId);
            }

            var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(UserId);
            if (roomUserByUserId != null && !roomUserByUserId.IsBot)
            {
                roomUserByUserId.GetClient().SendPacket(new YouAreControllerComposer(0));

                roomUserByUserId.RemoveStatus("flatctrl");
                roomUserByUserId.SetStatus("flatctrl", "0");
                roomUserByUserId.UpdateNeeded = true;
            }

            session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, UserId));

            if (room.UsersWithRights.Count <= 0)
            {
                session.SendPacket(new RoomRightsListComposer(room));
            }
            else
            {
                _ = room.UsersWithRights.Contains(UserId);
                session.SendPacket(new RoomRightsListComposer(room));
            }
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        RoomRightDao.DeleteList(dbClient, room.Id, userIds);
    }
}
