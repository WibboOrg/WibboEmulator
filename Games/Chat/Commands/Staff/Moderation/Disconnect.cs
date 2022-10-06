namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Disconnect : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser roomUser, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (TargetUser.GetUser().Rank >= session.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("action.notallowed", session.Langue));
        }
        else
        {
            TargetUser.Disconnect();
        }
    }
}
