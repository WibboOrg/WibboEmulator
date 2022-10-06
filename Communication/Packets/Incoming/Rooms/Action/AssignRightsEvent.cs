namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class AssignRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var UserId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (room.UsersWithRights.Contains(UserId))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.giverights.error", session.Langue));
        }
        else
        {
            var Userright = WibboEnvironment.GetUserById(UserId);
            if (Userright == null)
            {
                return;
            }

            room.UsersWithRights.Add(UserId);

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Insert(dbClient, room.Id, UserId);
            }

            session.SendPacket(new FlatControllerAddedComposer(room.Id, UserId, Userright.Username));

            var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(UserId);
            if (roomUserByUserId == null || roomUserByUserId.IsBot)
            {
                return;
            }

            roomUserByUserId.RemoveStatus("flatctrl");
            roomUserByUserId.SetStatus("flatctrl", "1");
            roomUserByUserId.UpdateNeeded = true;

            roomUserByUserId.GetClient().SendPacket(new YouAreControllerComposer(1));
        }
    }
}