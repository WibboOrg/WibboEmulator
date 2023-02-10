namespace WibboEmulator.Communication.RCON.Commands.User;
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

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userid);
        if (client == null)
        {
            return false;
        }

        client.SendNotification(parameters[2]);
        return true;
    }
}
