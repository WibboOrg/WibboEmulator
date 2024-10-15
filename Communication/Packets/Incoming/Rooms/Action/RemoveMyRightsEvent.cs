namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveMyRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (!Session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session))
        {
            return;
        }

        if (room.UsersWithRights.Contains(Session.User.Id))
        {
            var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
            if (user != null && !user.IsBot)
            {
                user.RemoveStatus("flatctrl");
                user.UpdateNeeded = true;

                user.Client.SendPacket(new YouAreNotControllerComposer());
            }

            using (var dbClient = DatabaseManager.Connection)
            {
                RoomRightDao.Delete(dbClient, room.Id, Session.User.Id);
            }

            if (room.UsersWithRights.Contains(Session.User.Id))
            {
                _ = room.UsersWithRights.Remove(Session.User.Id);
            }
        }
    }
}
