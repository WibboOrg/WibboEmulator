using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class MachineBan : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (string.IsNullOrEmpty(clientByUsername.MachineId))
            {
                return;
            }
            else if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", 788922000, "Votre compte � �t� banni par s�curit�", false, false);
            }
            else
            {
                string Raison = "";
                if (Params.Length > 2)
                {
                    Raison = CommandManager.MergeParams(Params, 2);
                }

                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, 788922000, Raison, true, true);
                UserRoom.SendWhisperChat("Tu viens de bannir " + clientByUsername.GetHabbo().Username + " pour la raison : " + Raison +" !");
                Session.Antipub(Raison, "<CMD>");
                return;
            }
        }
    }
}
