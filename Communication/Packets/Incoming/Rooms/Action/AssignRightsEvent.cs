namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class AssignRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        var userId = packet.PopInt();

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (room.UsersWithRights.Contains(userId))
        {
            session.SendNotification(LanguageManager.TryGetValue("user.giverights.error", session.Language));
        }
        else
        {
            var userRight = WibboEnvironment.GetUserById(userId);
            if (userRight == null)
            {
                return;
            }

            room.UsersWithRights.Add(userId);

            using (var dbClient = DatabaseManager.Connection)
            {
                RoomRightDao.Insert(dbClient, room.Id, userId);
            }

            session.SendPacket(new FlatControllerAddedComposer(room.Id, userId, userRight.Username));

            var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(userId);
            if (roomUserByUserId == null || roomUserByUserId.IsBot)
            {
                return;
            }

            roomUserByUserId.RemoveStatus("flatctrl");
            roomUserByUserId.SetStatus("flatctrl", "1");
            roomUserByUserId.UpdateNeeded = true;

            roomUserByUserId.Client.SendPacket(new YouAreControllerComposer(1));
        }
    }
}
