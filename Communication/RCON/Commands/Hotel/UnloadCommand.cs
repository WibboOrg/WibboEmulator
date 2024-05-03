namespace WibboEmulator.Communication.RCON.Commands.Hotel;

using WibboEmulator.Games.Rooms;

internal sealed class UnloadCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var roomId))
        {
            return false;
        }

        if (roomId == 0)
        {
            return false;
        }

        if (!RoomManager.TryGetRoom(roomId, out var room))
        {
            return true;
        }

        RoomManager.UnloadRoom(room);
        return true;
    }
}
