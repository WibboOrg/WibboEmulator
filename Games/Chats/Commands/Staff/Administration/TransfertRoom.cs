namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class TransfertRoom : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!room.CheckRights(session, true) && !session.User.HasPermission("transfert_all_room"))
        {
            return;
        }

        var username = parameters[1];

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        var userId = UserDao.GetIdByName(dbClient, username);
        if (userId == 0)
        {
            return;
        }

        var userTarget = WibboEnvironment.GetUserById(userId);

        RoomDao.UpdateOwnerByRoomId(dbClient, userTarget.Username, room.Id);

        room.RoomData.OwnerName = userTarget.Username;
        room.SendPacket(new RoomInfoUpdatedComposer(room.Id));

        var usersToReturn = room.RoomUserManager.GetRoomUsers().ToList();

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        foreach (var user in usersToReturn)
        {
            if (user == null || user.Client == null)
            {
                continue;
            }

            user.Client.SendPacket(new RoomForwardComposer(room.Id));
        }
    }
}
