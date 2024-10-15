namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveRightsEvent : IPacketEvent
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

        if (!room.CheckRights(Session, true))
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

            Session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, userId));

            if (room.UsersWithRights.Count <= 0)
            {
                Session.SendPacket(new RoomRightsListComposer(room));
            }
            else
            {
                _ = room.UsersWithRights.Contains(userId);
                Session.SendPacket(new RoomRightsListComposer(room));
            }
        }

        using var dbClient = DatabaseManager.Connection;
        RoomRightDao.DeleteAll(dbClient, room.Id, userIds);
    }
}
