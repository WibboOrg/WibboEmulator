namespace WibboEmulator.Communication.RCON.Commands.User;
using System.Text.RegularExpressions;
using WibboEmulator.Games.Moderation;

internal class EventHaCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userId))
        {
            return false;
        }

        if (userId == 0)
        {
            return false;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (client == null || client.GetUser().CurrentRoom == null)
        {
            return false;
        }

        var message = parameters[2];

        ModerationManager.LogStaffEntry(client.GetUser().Id, client.GetUser().Username, 0, string.Empty, "eventha", string.Format("WbTool eventha: {0}", message));
        if (client.Antipub(message, "<eventalert>", client.GetUser().CurrentRoom.Id))
        {
            return false;
        }

        if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
        {
            return false;
        }

        message = message.Replace("<", "&lt;").Replace(">", "&gt;");

        message = new Regex(@"\[b\](.*?)\[\/b\]").Replace(message, "<b>$1</b>");
        message = new Regex(@"\[i\](.*?)\[\/i\]").Replace(message, "<i>$1</i>");
        message = new Regex(@"\[u\](.*?)\[\/u\]").Replace(message, "<u>$1</u>");

        var alertMessage = message + "\r\n- " + client.GetUser().Username;
        WibboEnvironment.GetGame().GetGameClientManager().SendSuperNotif("Message des Staffs", alertMessage, "game_promo_small", "event:navigator/goto/" + client.GetUser().CurrentRoom.Id, "Je veux y accéder!");
        client.GetUser().CurrentRoom.CloseFullRoom = true;

        return true;
    }
}
