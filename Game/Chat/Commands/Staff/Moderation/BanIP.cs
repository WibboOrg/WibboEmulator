using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class BanIP : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser userRoom, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                return;
            }

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(parameters[1]);
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

            WibboEnvironment.GetGame().GetClientManager().BanUser(TargetUser, session.GetUser().Username, 788922000, reason, true, false);
            session.SendWhisper("Tu as banIP " + TargetUser.GetUser().Username + " pour" + reason + "!");
            session.Antipub(reason, "<CMD>");

            if (TargetUser.GetUser().Rank > 5 && session.GetUser().Rank < 12)
            {
                WibboEnvironment.GetGame().GetClientManager().BanUser(session, "Robot", 788922000, "Votre compte � �t� banni par s�curit�", false, false);
            }
        }
    }
}
