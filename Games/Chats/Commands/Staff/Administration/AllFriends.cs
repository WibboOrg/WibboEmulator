namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class AllFriends : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        foreach (var user in GameClientManager.Clients.ToList())
        {
            if (user == null)
            {
                continue;
            }

            if (user.User == null)
            {
                continue;
            }

            if (user.User.Messenger == null)
            {
                continue;
            }

            if (!user.User.Messenger.FriendshipExists(userRoom.UserId))
            {
                user.User.Messenger.OnNewFriendship(userRoom.UserId);
            }

            if (!Session.User.Messenger.FriendshipExists(user.User.Id))
            {
                Session.User.Messenger.OnNewFriendship(user.User.Id);
            }
        }
    }
}
