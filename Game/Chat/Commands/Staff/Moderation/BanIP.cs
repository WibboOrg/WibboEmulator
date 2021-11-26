using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class BanIP : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser userRoom, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                return;
            }

            Client clientTrajet = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(parameters[1]);
            if (clientTrajet == null || clientTrajet.GetHabbo() == null)
            {
                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
                return;
            }

            if (clientTrajet.GetHabbo().Rank >= session.GetHabbo().Rank)
            {
                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
                return;
            }

            string reason = "";
            if (parameters.Length > 2)
            {
                reason = CommandManager.MergeParams(parameters, 2);
            }

            ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientTrajet, session.GetHabbo().Username, 788922000, reason, true, false);
            userRoom.SendWhisperChat("Tu as banIP " + clientTrajet.GetHabbo().Username + " pour" + reason + "!");
            session.Antipub(reason, "<CMD>");

            if (clientTrajet.GetHabbo().Rank > 5 && session.GetHabbo().Rank < 12)
            {
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(session, "Robot", 788922000, "Votre compte � �t� banni par s�curit�", false, false);
            }
        }
    }
}
