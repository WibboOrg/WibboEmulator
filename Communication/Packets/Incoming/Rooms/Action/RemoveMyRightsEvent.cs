namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RemoveMyRightsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        if (!Room.CheckRights(session))
        {
            return;
        }

        if (Room.UsersWithRights.Contains(session.GetUser().Id))
        {
            var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (User != null && !User.IsBot)
            {
                User.RemoveStatus("flatctrl");
                User.UpdateNeeded = true;

                User.GetClient().SendPacket(new YouAreNotControllerComposer());
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Delete(dbClient, Room.Id, session.GetUser().Id);
            }

            if (Room.UsersWithRights.Contains(session.GetUser().Id))
            {
                Room.UsersWithRights.Remove(session.GetUser().Id);
            }
        }
    }
}
