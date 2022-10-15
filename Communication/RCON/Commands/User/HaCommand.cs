namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Moderation;

internal class HaCommand : IRCONCommand
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
        if (client == null)
        {
            return false;
        }

        var message = parameters[2];

        ModerationManager.LogStaffEntry(client.User.Id, client.User.Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", message));
        if (client.Antipub(message, "<alert>"))
        {
            return false;
        }

        WibboEnvironment
            .GetGame()
            .GetGameClientManager()
            .SendMessage(new BroadcastMessageAlertComposer(WibboEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", client.Langue) + "\r\n" + message + "\r\n- " + client.User.Username));
        return true;
    }
}
