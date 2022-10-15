namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllFriends : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        foreach (var user in WibboEnvironment.GetGame().GetGameClientManager().GetClients)
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

            if (!session.User.Messenger.FriendshipExists(user.User.Id))
            {
                session.User.Messenger.OnNewFriendship(user.User.Id);
            }
        }
    }
}
