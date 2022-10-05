namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

internal class HandItem : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        int.TryParse(Params[1], out var handitemid);
        if (handitemid < 0)
        {
            return;
        }

        UserRoom.CarryItem(handitemid);
    }
}
