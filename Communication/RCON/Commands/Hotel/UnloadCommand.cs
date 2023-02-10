namespace WibboEmulator.Communication.RCON.Commands.Hotel;
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

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return false;
        }

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
        return true;
    }
}
