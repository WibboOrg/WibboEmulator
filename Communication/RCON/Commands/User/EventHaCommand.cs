using WibboEmulator.Game.Clients;
using System.Text.RegularExpressions;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class EventHaCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parameters[1], out int Userid))
            {
                return false;
            }

            if (Userid == 0)
            {
                return false;
            }

            Client Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null || Client.GetUser().CurrentRoom == null)
            {
                return false;
            }

            string Message = parameters[2];

            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetUser().Id, Client.GetUser().Username, 0, string.Empty, "eventha", string.Format("WbTool eventha: {0}", Message));
            if (Client.Antipub(Message, "<eventalert>", Client.GetUser().CurrentRoom.Id))
            {
                return false;
            }

            if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
            {
                return false;
            }

            Message = Message.Replace("<", "&lt;").Replace(">", "&gt;");

            Message = new Regex(@"\[b\](.*?)\[\/b\]").Replace(Message, "<b>$1</b>");
            Message = new Regex(@"\[i\](.*?)\[\/i\]").Replace(Message, "<i>$1</i>");
            Message = new Regex(@"\[u\](.*?)\[\/u\]").Replace(Message, "<u>$1</u>");

            string AlertMessage = Message + "\r\n- " + Client.GetUser().Username;
            WibboEnvironment.GetGame().GetClientManager().SendSuperNotif("Message des Staffs", AlertMessage, "game_promo_small", "event:navigator/goto/" + Client.GetUser().CurrentRoom.Id, "Je veux y accéder!", true, true);
            Client.GetUser().CurrentRoom.CloseFullRoom = true;

            return true;
        }
    }
}
