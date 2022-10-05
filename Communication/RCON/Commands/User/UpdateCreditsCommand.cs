namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos;

internal class UpdateCreditsCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var Userid))
        {
            return false;
        }

        if (Userid == 0)
        {
            return false;
        }

        var Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Userid);
        if (Client == null)
        {
            return false;
        }

        int credits;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            credits = UserDao.GetCredits(dbClient, Client.GetUser().Id);
        }

        Client.GetUser().Credits = credits;
        Client.SendPacket(new CreditBalanceComposer(Client.GetUser().Credits));

        return true;
    }
}
