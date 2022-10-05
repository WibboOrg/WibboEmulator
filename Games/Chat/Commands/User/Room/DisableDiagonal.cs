namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableDiagonal : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) => Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
}
