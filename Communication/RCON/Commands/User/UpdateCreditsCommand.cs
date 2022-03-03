using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.RCON.Commands.User
{
    internal class UpdateCreditsCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(parameters[1], out int Userid))
            {
                return false;
            }

            if (Userid == 0)
            {
                return false;
            }

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            int credits;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                credits = UserDao.GetCredits(dbClient, Client.GetUser().Id);
            }

            Client.GetUser().Credits = credits;
            Client.SendPacket(new CreditBalanceComposer(Client.GetUser().Credits));

            return true;
        }
    }
}
