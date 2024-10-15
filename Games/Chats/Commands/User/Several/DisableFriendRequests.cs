namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableFriendRequests : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (Session.User.HasFriendRequestsDisabled)
        {
            Session.User.HasFriendRequestsDisabled = false;
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.textamigo.true", Session.Language));
        }
        else
        {
            Session.User.HasFriendRequestsDisabled = true;
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.textamigo.false", Session.Language));
        }
    }
}
