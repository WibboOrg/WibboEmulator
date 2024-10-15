namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class UnloadRoom : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var roomId))
        {
            return;
        }

        if (!RoomManager.TryGetRoom(roomId, out var roomTarget))
        {
            return;
        }

        RoomManager.UnloadRoom(roomTarget);
    }
}
