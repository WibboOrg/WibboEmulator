namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class RemoveMyRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!session.User.InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        if (room.UsersWithRights.Contains(session.User.Id))
        {
            var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
            if (user != null && !user.IsBot)
            {
                user.RemoveStatus("flatctrl");
                user.UpdateNeeded = true;

                user.Client.SendPacket(new YouAreNotControllerComposer());
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Delete(dbClient, room.Id, session.User.Id);
            }

            if (room.UsersWithRights.Contains(session.User.Id))
            {
                _ = room.UsersWithRights.Remove(session.User.Id);
            }
        }
    }
}
