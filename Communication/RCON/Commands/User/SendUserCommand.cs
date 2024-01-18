namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class SendUserCommand : IRCONCommand
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

        if (!int.TryParse(parameters[2], out var roomId))
        {
            return false;
        }

        if (roomId == 0)
        {
            return false;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (client == null || client.User == null)
        {
            return true;
        }

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
        if (roomData == null)
        {
            return true;
        }

        client.SendPacket(new GetGuestRoomResultComposer(client, roomData, false, true));
        return true;
    }
}
