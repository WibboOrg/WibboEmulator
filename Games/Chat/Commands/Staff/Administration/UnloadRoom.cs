namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class UnloadRoom : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var roomId))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var roomTarget))
        {
            return;
        }

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(roomTarget);
    }
}
