using Butterfly.Game.GameClients;namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class BanIP : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length < 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }            if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));            }            else            {                string Raison = "";                if (Params.Length > 2)
                {
                    Raison = CommandManager.MergeParams(Params, 2);
                }

                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, 788922000, Raison, true, false);                UserRoom.SendWhisperChat("Tu as banIP " + clientByUsername.GetHabbo().Username + " pour" + Raison + "!");                Session.Antipub(Raison, "<CMD>");

                if (clientByUsername.GetHabbo().Rank > 5 && Session.GetHabbo().Rank < 12)
                {
                    ButterflyEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", 788922000, "Votre compte à été banni par sécurité", false, false);
                }            }        }    }}