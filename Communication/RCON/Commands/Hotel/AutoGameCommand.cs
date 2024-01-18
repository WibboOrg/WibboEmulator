namespace WibboEmulator.Communication.RCON.Commands.Hotel;

internal sealed class AutoGameCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 1)
        {
            return false;
        }

        WibboEnvironment.GetGame().GetAnimationManager().ForceDisabled(parameters[0] == "1");
        return true;
    }
}
