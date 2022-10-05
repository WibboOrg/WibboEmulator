namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;

internal class UpdateLimitCoinsCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userId))
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        var Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (Client == null)
        {
            return false;
        }

        if (!int.TryParse(parameters[2], out var amount))
        {
            return false;
        }

        if (amount == 0)
        {
            return false;
        }

        Client.GetUser().LimitCoins += amount;
        Client.SendPacket(new ActivityPointNotificationComposer(Client.GetUser().LimitCoins, 0, 55));

        return true;
    }
}
