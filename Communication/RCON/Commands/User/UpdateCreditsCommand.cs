using Wibbo.Communication.Packets.Outgoing.Inventory.Purse;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.RCON.Commands.User
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

            Client Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            int credits;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                credits = UserDao.GetCredits(dbClient, Client.GetUser().Id);
            }

            Client.GetUser().Credits = credits;
            Client.SendPacket(new CreditBalanceComposer(Client.GetUser().Credits));

            return true;
        }
    }
}
