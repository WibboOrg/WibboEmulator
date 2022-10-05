namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class UnMute : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else
        {
            var user = TargetUser.GetUser();

            user.SpamProtectionTime = 10;
            user.SpamEnable = true;
        }
    }
}
