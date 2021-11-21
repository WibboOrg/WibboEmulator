using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.RCON.Commands.User
{
    internal class UpdatePointsCommand : IRCONCommand
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

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
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

            Client.GetHabbo().WibboPoints += NbWb;
            Client.SendPacket(new ActivityPointsComposer(Client.GetHabbo().WibboPoints));

            return true;
        }
    }
}
