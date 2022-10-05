namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal class SendUserCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var Userid))
        {
            return false;
        }

        if (Userid == 0)
        {
            return false;
        }

        var Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Userid);
        if (Client == null || Client.GetUser() == null)
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

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
        if (roomData == null)
        {
            return false;
        }

        Client.SendPacket(new GetGuestRoomResultComposer(Client, roomData, false, true));
        return true;
    }
}
