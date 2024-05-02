namespace WibboEmulator.Communication.RCON.Commands.Hotel;

using WibboEmulator.Games.Animations;

internal sealed class AutoGameCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 1)
        {
            return false;
        }

        AnimationManager.ForceDisabled(parameters[0] == "1");
        return true;
    }
}
