using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class BanIP : IChatCommand
    {
        public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                return;
            }

            GameClient TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
                return;
            }

            if (TargetUser.GetUser().Rank >= session.GetUser().Rank)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
                return;
            }

            string reason = "";
            if (parameters.Length > 2)
            {
                reason = CommandManager.MergeParams(parameters, 2);
            }

            session.SendWhisper("Tu as banIP " + TargetUser.GetUser().Username + " pour " + reason + "!");

            if (TargetUser.GetUser().Rank > 5 && session.GetUser().Rank < 12)
            {
                WibboEnvironment.GetGame().GetGameClientManager().BanUser(session, "Robot", 788922000, "Votre compte à été banni par sécurité", false, false);
            }
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(TargetUser, session.GetUser().Username, 788922000, reason, true, false);
            session.Antipub(reason, "<CMD>");
        }
    }
}
