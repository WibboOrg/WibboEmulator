namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SetMax : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 1)
        {
            return;
        }

        int.TryParse(Params[1], out var MaxUsers);

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
