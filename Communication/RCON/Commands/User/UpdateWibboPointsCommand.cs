using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class UpdateWibboPointsCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
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

            GameClient Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            if (!int.TryParse(parameters[2], out int NbWb))
            {
                return false;
            }

            if (NbWb == 0)
            {
                return false;
            }

            Client.GetUser().WibboPoints += NbWb;
            Client.SendPacket(new ActivityPointNotificationComposer(Client.GetUser().WibboPoints, 0, 105));

            return true;
        }
    }
}
