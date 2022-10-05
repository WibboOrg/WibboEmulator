namespace WibboEmulator.Communication.RCON.Commands.User;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;

internal class AddWinwinCommand : IRCONCommand
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

        if (!int.TryParse(parameters[2], out var Winwin))
        {
            return false;
        }

        if (Winwin == 0)
        {
            return false;
        }

        Client.GetUser().AchievementPoints = Client.GetUser().AchievementPoints + Winwin;
        Client.SendPacket(new AchievementScoreComposer(Client.GetUser().AchievementPoints));

        return true;
    }
}
