namespace WibboEmulator.Communication.RCON.Commands.User;
using System.Text.RegularExpressions;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.GameClients;
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

        var client = GameClientManager.GetClientByUserID(userId);
        if (client == null || client.User.Room == null)
        {
            return true;
        }

        var message = parameters[2];

        ModerationManager.LogStaffEntry(client.User.Id, client.User.Username, 0, string.Empty, "eventha", string.Format("WbTool eventha: {0}", message));

        if (!AnimationManager.AllowAnimation)
        {
            return true;
        }

        message = message.Replace("<", "&lt;").Replace(">", "&gt;");

        message = MyRegex().Replace(message, "<b>$1</b>");
        message = MyRegex1().Replace(message, "<i>$1</i>");
        message = MyRegex2().Replace(message, "<u>$1</u>");

        var alertMessage = message + "\r\n- " + client.User.Username;
        GameClientManager.SendSuperNotification("Message des Staffs", alertMessage, "game_promo_small", "event:navigator/goto/" + client.User.Room.Id, "Je veux y accéder!");
        client.User.Room.CloseFullRoom = true;

        return true;
    }

    [GeneratedRegex("\\[b\\](.*?)\\[\\/b\\]")]
    private static partial Regex MyRegex();
    [GeneratedRegex("\\[i\\](.*?)\\[\\/i\\]")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("\\[u\\](.*?)\\[\\/u\\]")]
    private static partial Regex MyRegex2();
}
