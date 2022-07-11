using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class UserAlertCommand : IRCONCommand
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

            Client Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            Client.SendNotification(parameters[2]);
            return true;
        }
    }
}
