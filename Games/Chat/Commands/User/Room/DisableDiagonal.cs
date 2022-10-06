namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableDiagonal : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters) => Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
}
