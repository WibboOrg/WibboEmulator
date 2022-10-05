namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableFriendRequests : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (session.GetUser().HasFriendRequestsDisabled)
        {
            session.GetUser().HasFriendRequestsDisabled = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.textamigo.true", session.Langue));
        }
        else
        {
            session.GetUser().HasFriendRequestsDisabled = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.textamigo.false", session.Langue));
        }
    }
}
