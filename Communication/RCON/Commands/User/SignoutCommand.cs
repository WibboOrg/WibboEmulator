using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class SignOutCommand : IRCONCommand
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

            if (Userid <= 0)
            {
                return false;
            }

            GameClient Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            Client.Disconnect();
            return true;
        }
    }
}
