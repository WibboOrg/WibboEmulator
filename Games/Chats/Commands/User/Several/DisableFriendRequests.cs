namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableFriendRequests : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.HasFriendRequestsDisabled)
        {
            session.User.HasFriendRequestsDisabled = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.textamigo.true", session.Langue));
        }
        else
        {
            session.User.HasFriendRequestsDisabled = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.textamigo.false", session.Langue));
        }
    }
}
