namespace WibboEmulator.Communication.RCON.Commands.User;
using System.Text.RegularExpressions;

internal class EventHaCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var Userid))
        {
            return false;
        }

        if (Userid == 0)
        {
            return false;
        }

        var Client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Userid);
        if (Client == null || Client.GetUser().CurrentRoom == null)
        {
            return false;
        }

        var Message = parameters[2];

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

        var AlertMessage = Message + "\r\n- " + Client.GetUser().Username;
        WibboEnvironment.GetGame().GetGameClientManager().SendSuperNotif("Message des Staffs", AlertMessage, "game_promo_small", "event:navigator/goto/" + Client.GetUser().CurrentRoom.Id, "Je veux y accéder!", true, true);
        Client.GetUser().CurrentRoom.CloseFullRoom = true;

        return true;
    }
}
