namespace WibboEmulator.Communication.RCON.Commands.User;

using WibboEmulator.Games.GameClients;

internal sealed class UserAlertCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userid))
        {
            return false;
        }

        if (userid == 0)
        {
            return false;
        }

        var client = GameClientManager.GetClientByUserID(userid);
        if (client == null)
        {
            return true;
        }

        var message = parameters[2];

        if (message.Length < 3)
        {
            return false;
        }

        client.SendNotification(message);
        return true;
    }
}
