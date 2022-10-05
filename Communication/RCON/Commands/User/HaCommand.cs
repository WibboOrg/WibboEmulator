namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;

internal class HaCommand : IRCONCommand
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
        if (Client == null)
        {
            return false;
        }

        var Message = parameters[2];

        WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetUser().Id, Client.GetUser().Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", Message));
        if (Client.Antipub(Message, "<alert>"))
        {
            return false;
        }

        WibboEnvironment
            .GetGame()
            .GetGameClientManager()
            .SendMessage(new BroadcastMessageAlertComposer(WibboEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", Client.Langue) + "\r\n" + Message + "\r\n- " + Client.GetUser().Username));
        return true;
    }
}
