namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;

internal sealed class UpdateWibboPointsCommand : IRCONCommand
{
    public bool TryExecute(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parameters[1], out var userid))
        {
            return false;
        }

        if (userid == 0)
        {
            return false;
        }

        if (!int.TryParse(parameters[2], out var amount))
        {
            return false;
        }

        if (amount == 0)
        {
            return false;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userid);
        if (client == null)
        {
            return true;
        }

        client.User.WibboPoints += amount;
        client.SendPacket(new ActivityPointNotificationComposer(client.User.WibboPoints, 0, 105));

        return true;
    }
}
