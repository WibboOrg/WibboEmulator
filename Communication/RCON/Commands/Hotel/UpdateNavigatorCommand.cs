namespace WibboEmulator.Communication.RCON.Commands.Hotel;
internal class UpdateNavigatorCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            WibboEnvironment.GetGame().GetNavigator().Init(dbClient);
        }

        return true;
    }
}
