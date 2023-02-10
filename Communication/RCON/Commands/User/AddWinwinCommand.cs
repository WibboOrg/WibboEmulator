namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;

internal sealed class AddWinwinCommand : IRCONCommand
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

        if (!int.TryParse(parameters[2], out var winwin))
        {
            return false;
        }

        if (winwin == 0)
        {
            return false;
        }

        client.
        User.AchievementPoints = client.User.AchievementPoints + winwin;
        client.SendPacket(new AchievementScoreComposer(client.User.AchievementPoints));

        return true;
    }
}
