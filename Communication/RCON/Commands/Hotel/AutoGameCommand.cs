namespace WibboEmulator.Communication.RCON.Commands.Hotel;

internal sealed class AutoGameCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        WibboEnvironment.GetGame().GetAnimationManager().ForceDisabled(parameters[0] == "1");
        return true;
    }
}
