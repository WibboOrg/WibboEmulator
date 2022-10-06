namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class HandItem : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var handitemid);
        if (handitemid < 0)
        {
            return;
        }

        UserRoom.CarryItem(handitemid);
    }
}
