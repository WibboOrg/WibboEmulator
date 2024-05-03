namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

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

        var client = GameClientManager.GetClientByUserID(userId);
        if (client == null || client.User == null)
        {
            return true;
        }

        var roomData = RoomManager.GenerateRoomData(roomId);
        if (roomData == null)
        {
            return true;
        }

        client.SendPacket(new GetGuestRoomResultComposer(client, roomData, false, true));
        return true;
    }
}
