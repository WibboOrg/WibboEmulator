namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableDiagonal : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters) => room.GetGameMap().DiagonalEnabled = !room.GetGameMap().DiagonalEnabled;
}
