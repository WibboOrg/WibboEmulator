namespace WibboEmulator.Games.Chat.Commands.User.Room;

using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SetMax : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        if (int.TryParse(parameters[1], out var maxUsers))
        {
            return;
        }

        if ((maxUsers > 75 || maxUsers <= 0) && !session.GetUser().HasPermission("perm_mod"))
        {
            room.RoomData.UsersMax = 75;
        }
        else
        {
            room.RoomData.UsersMax = maxUsers;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        RoomDao.UpdateUsersMax(dbClient, room.Id, maxUsers);
    }
}
