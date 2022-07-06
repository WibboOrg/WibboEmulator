using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class UpdateLimitCoinsCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parameters[1], out int userId))
            {
                return false;
            }

            if (userId == 0)
            {
                return false;
            }

            Client Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (Client == null)
            {
                return false;
            }

            if (!int.TryParse(parameters[2], out int amount))
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
}
