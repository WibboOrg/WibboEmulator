using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class BanIP : IChatCommand    {        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (Params.Length < 2)
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

                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, 788922000, Raison, true, false);                Session.Antipub(Raison, "<CMD>");

                if (clientByUsername.GetHabbo().Rank > 5 && Session.GetHabbo().Rank < 12)
                {
                    ButterflyEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", 788922000, "Votre compte à été banni par sécurité", false, false);
                }            }        }    }}