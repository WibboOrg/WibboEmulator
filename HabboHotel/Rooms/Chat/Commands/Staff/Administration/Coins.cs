using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Coins : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            Room currentRoom = Session.GetHabbo().CurrentRoom;            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername != null)            {                if (int.TryParse(Params[2], out int result))                {                    clientByUsername.GetHabbo().Credits = clientByUsername.GetHabbo().Credits + result;                    clientByUsername.GetHabbo().UpdateCreditsBalance();                    clientByUsername.SendNotification(Session.GetHabbo().Username + ButterflyEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage1", Session.Langue) + result.ToString() + ButterflyEnvironment.GetLanguageManager().TryGetValue("coins.awardmessage2", Session.Langue));                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("coins.updateok", Session.Langue));                }                else
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));
                }
            }            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }        }    }}