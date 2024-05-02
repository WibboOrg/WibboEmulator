namespace WibboEmulator.Communication.RCON.Commands.Hotel;

internal sealed class ShutdownCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        WibboEnvironment.PerformShutDown();

        return true;
    }
}
