namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class HandItem : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (userRoom.Team != TeamType.NONE || userRoom.InGame)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var handitemid);
        if (handitemid < 0)
        {
            return;
        }

        userRoom.CarryItem(handitemid);
    }
}
