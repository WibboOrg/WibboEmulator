using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.RCON.Commands.User
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

            GameClient Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
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
