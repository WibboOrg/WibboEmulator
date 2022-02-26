using Butterfly.Database.Interfaces;

namespace Butterfly.Communication.RCON.Commands.Hotel
{
    internal class UpdateNavigatorCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                ButterflyEnvironment.GetGame().GetNavigator().Init(dbClient);

            return true;
        }
    }
}
