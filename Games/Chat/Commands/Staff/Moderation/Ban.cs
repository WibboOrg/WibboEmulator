using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Ban : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            GameClient TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            if (TargetUser.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                return;
            }

            int.TryParse(Params.Length >= 3 ? Params[2] : "0", out int num);
            if (num <= 600)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
            }
            else
            {
                string Raison = CommandManager.MergeParams(Params, 3);
                Session.SendWhisper("Tu as bannit " + TargetUser.GetUser().Username + " pour " + Raison + "!");

                WibboEnvironment.GetGame().GetClientManager().BanUser(TargetUser, Session.GetUser().Username, num, Raison, false, false);
                Session.Antipub(Raison, "<CMD>", Room.Id);
            }
        }
    }
}
