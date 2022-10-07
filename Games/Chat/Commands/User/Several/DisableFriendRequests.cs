namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableFriendRequests : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
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
