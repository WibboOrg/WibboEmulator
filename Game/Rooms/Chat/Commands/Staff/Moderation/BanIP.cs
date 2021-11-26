using Butterfly.Game.Clients;namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class BanIP : IChatCommand    {        public void Execute(Client session, Room room, RoomUser userRoom, string[] inputs)        {            if (inputs.Length < 2)
            {
                return;
            }

            Client clientTrajet = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(inputs[1]);            if (clientTrajet == null || clientTrajet.GetHabbo() == null)
            {                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
                return;
            }            if (clientTrajet.GetHabbo().Rank >= session.GetHabbo().Rank)            {                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));                return;            }

            string reason = "";
            if (inputs.Length > 2)
            {
                reason = CommandManager.MergeParams(inputs, 2);
            }

            ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientTrajet, session.GetHabbo().Username, 788922000, reason, true, false);
            userRoom.SendWhisperChat("Tu as banIP " + clientTrajet.GetHabbo().Username + " pour" + reason + "!");
            session.Antipub(reason, "<CMD>");

            if (clientTrajet.GetHabbo().Rank > 5 && session.GetHabbo().Rank < 12)
            {
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(session, "Robot", 788922000, "Votre compte à été banni par sécurité", false, false);
            }        }    }}