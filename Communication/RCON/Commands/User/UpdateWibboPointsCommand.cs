namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;

internal class UpdateWibboPointsCommand : IRCONCommand
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

        if (!int.TryParse(parameters[2], out var NbWb))
        {
            return false;
        }

        if (NbWb == 0)
        {
            return false;
        }

        Client.GetUser().WibboPoints += NbWb;
        Client.SendPacket(new ActivityPointNotificationComposer(Client.GetUser().WibboPoints, 0, 105));

        return true;
    }
}
