namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class RemoveMyRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        if (room.UsersWithRights.Contains(session.GetUser().Id))
        {
            var user = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (user != null && !user.IsBot)
            {
                user.RemoveStatus("flatctrl");
                user.UpdateNeeded = true;

                user.Client.SendPacket(new YouAreNotControllerComposer());
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Delete(dbClient, room.Id, session.GetUser().Id);
            }

            if (room.UsersWithRights.Contains(session.GetUser().Id))
            {
                _ = room.UsersWithRights.Remove(session.GetUser().Id);
            }
        }
    }
}
