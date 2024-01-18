namespace WibboEmulator.Communication.RCON.Commands.User;
internal sealed class SignOutCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userId))
        {
            return false;
        }

        if (userId <= 0)
        {
            return false;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (client == null)
        {
            return true;
        }

        client.Disconnect();
        return true;
    }
}
