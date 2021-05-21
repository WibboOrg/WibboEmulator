using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.RCON.Commands.User
{
    internal class AddWinwinCommand : IRCONCommand
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

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            if (!int.TryParse(parameters[2], out int Winwin))
            {
                return false;
            }

            if (Winwin == 0)
            {
                return false;
            }

            Client.GetHabbo().AchievementPoints = Client.GetHabbo().AchievementPoints + Winwin;
            Client.SendPacket(new AchievementScoreComposer(Client.GetHabbo().AchievementPoints));

            return true;
        }
    }
}
