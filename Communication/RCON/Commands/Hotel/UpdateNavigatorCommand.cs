using Wibbo.Database.Interfaces;

namespace Wibbo.Communication.RCON.Commands.Hotel
{
    internal class UpdateNavigatorCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                WibboEnvironment.GetGame().GetNavigator().Init(dbClient);

            return true;
        }
    }
}
