using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class MachineBan : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client clientByUsername = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (string.IsNullOrEmpty(clientByUsername.MachineId))
            {
                return;
            }
            else if (clientByUsername.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                WibboEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", 788922000, "Votre compte a été banni par sécurité", false, false);
            }
            else
            {
                string Raison = "";
                if (Params.Length > 2)
                {
                    Raison = CommandManager.MergeParams(Params, 2);
                }

                WibboEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetUser().Username, 788922000, Raison, true, true);
                Session.SendWhisper("Tu viens de bannir " + clientByUsername.GetUser().Username + " pour la raison : " + Raison +" !");
                Session.Antipub(Raison, "<CMD>");
                return;
            }
        }
    }
}
