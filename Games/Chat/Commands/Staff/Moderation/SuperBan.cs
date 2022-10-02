using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class SuperBan : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            GameClient TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                WibboEnvironment.GetGame().GetGameClientManager().BanUser(Session, "Robot", 788922000, "Votre compte a été banni par sécurité !", false, false);
            }
            else
            {
                int num = 788922000;
                if (Params.Length >= 3)
                {
                    int.TryParse(Params[2], out num);
                }

                if (num <= 600)
                {
                    Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
                }
                else
                {
                    string Raison = CommandManager.MergeParams(Params, 3);
                    Session.SendWhisper("Tu as SuperBan " + TargetUser.GetUser().Username + " pour" + Raison + "!");

                    WibboEnvironment.GetGame().GetGameClientManager().BanUser(TargetUser, Session.GetUser().Username, num, Raison, false, false);
                    Session.Antipub(Raison, "<CMD>");
                }
            }
        }
    }
}
