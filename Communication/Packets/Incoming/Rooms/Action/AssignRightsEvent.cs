namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class AssignRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var userId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (room.UsersWithRights.Contains(userId))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.giverights.error", session.Langue));
        }
        else
        {
            var userRight = WibboEnvironment.GetUserById(userId);
            if (userRight == null)
            {
                return;
            }

            room.UsersWithRights.Add(userId);

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Insert(dbClient, room.Id, userId);
            }

            session.SendPacket(new FlatControllerAddedComposer(room.Id, userId, userRight.Username));

            var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(userId);
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
