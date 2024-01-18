namespace WibboEmulator.Communication.RCON.Commands.User;
using System.Text.RegularExpressions;
using WibboEmulator.Games.Moderations;

internal sealed partial class EventHaCommand : IRCONCommand
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
        if (client == null || client.User.CurrentRoom == null)
        {
            return true;
        }

        var message = parameters[2];

        ModerationManager.LogStaffEntry(client.User.Id, client.User.Username, 0, string.Empty, "eventha", string.Format("WbTool eventha: {0}", message));

        if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
        {
            return true;
        }

        message = message.Replace("<", "&lt;").Replace(">", "&gt;");

        message = MyRegex().Replace(message, "<b>$1</b>");
        message = MyRegex1().Replace(message, "<i>$1</i>");
        message = MyRegex2().Replace(message, "<u>$1</u>");

        var alertMessage = message + "\r\n- " + client.User.Username;
        WibboEnvironment.GetGame().GetGameClientManager().SendSuperNotif("Message des Staffs", alertMessage, "game_promo_small", "event:navigator/goto/" + client.User.CurrentRoom.Id, "Je veux y accéder!");
        client.User.CurrentRoom.CloseFullRoom = true;

        return true;
    }

    [GeneratedRegex("\\[b\\](.*?)\\[\\/b\\]")]
    private static partial Regex MyRegex();
    [GeneratedRegex("\\[i\\](.*?)\\[\\/i\\]")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("\\[u\\](.*?)\\[\\/u\\]")]
    private static partial Regex MyRegex2();
}
