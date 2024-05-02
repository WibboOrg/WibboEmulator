namespace WibboEmulator.Communication.RCON.Commands.Hotel;

using WibboEmulator.Database;
using WibboEmulator.Games.Navigators;

internal sealed class UpdateNavigatorCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        using var dbClient = DatabaseManager.Connection;
        NavigatorManager.Initialize(dbClient);

        return true;
    }
}
