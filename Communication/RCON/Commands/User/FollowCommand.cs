namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class FollowCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userId))
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        if (!int.TryParse(parameters[2], out var userIdTwo))
        {
            return false;
        }

        if (userIdTwo == 0)
        {
            return false;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (client == null)
        {
            return true;
        }

        var clientTwo = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userIdTwo);
        if (clientTwo == null || clientTwo.User == null)
        {
            return true;
        }

        var room = clientTwo.User.CurrentRoom;
        if (room == null)
        {
            return true;
        }

        client.SendPacket(new GetGuestRoomResultComposer(client, room.RoomData, false, true));
        return true;
    }
}
