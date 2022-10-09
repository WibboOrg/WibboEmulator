namespace WibboEmulator.Games.Chat.Commands.User.Room;
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

        _ = int.TryParse(parameters[1], out var maxUsers);

        if ((maxUsers > 75 || maxUsers <= 0) && !session.GetUser().HasPermission("perm_mod"))
        {
            room.SetMaxUsers(75);
        }
        else
        {
            room.SetMaxUsers(maxUsers);
        }
    }
}
