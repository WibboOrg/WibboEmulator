namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users;

internal sealed class AssignRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        var userId = packet.PopInt();

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        if (room.UsersWithRights.Contains(userId))
        {
            Session.SendNotification(LanguageManager.TryGetValue("user.giverights.error", Session.Language));
        }
        else
        {
            var userRight = UserManager.GetUserById(userId);
            if (userRight == null)
            {
                return;
            }

            room.UsersWithRights.Add(userId);

            using (var dbClient = DatabaseManager.Connection)
            {
                RoomRightDao.Insert(dbClient, room.Id, userId);
            }

            Session.SendPacket(new FlatControllerAddedComposer(room.Id, userId, userRight.Username));

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
