using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class SuperBan : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", 788922000, "Votre compte a été banni par sécurité !", false, false);
            }
            else
            {
                int num = 788922000;
                if (Params.Length == 3)
                {
                    int.TryParse(Params[2], out num);
                }

                if (num <= 600)
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
                }
                else
                {
                    string Raison = CommandManager.MergeParams(Params, 3);
                    ButterflyEnvironment.GetGame().GetClientManager().BanUser(TargetUser, Session.GetUser().Username, num, Raison, false, false);
                    UserRoom.SendWhisperChat("Tu as SuperBan " + TargetUser.GetUser().Username + " pour" + Raison + "!");
                    Session.Antipub(Raison, "<CMD>");
                }
            }

        }
    }
}
