namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SetMax : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var MaxUsers);

        if ((MaxUsers > 75 || MaxUsers <= 0) && !session.GetUser().HasPermission("perm_mod"))
        {
            Room.SetMaxUsers(75);
        }
        else
        {
            Room.SetMaxUsers(MaxUsers);
        }
    }
}
