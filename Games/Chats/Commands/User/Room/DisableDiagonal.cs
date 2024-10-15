namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableDiagonal : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) => room.GameMap.DiagonalEnabled = !room.GameMap.DiagonalEnabled;
}
