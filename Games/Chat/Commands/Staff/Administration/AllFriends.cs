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

            if (user.GetUser() == null)
            {
                continue;
            }

            if (user.GetUser().Messenger == null)
            {
                continue;
            }

            if (!user.GetUser().Messenger.FriendshipExists(userRoom.UserId))
            {
                user.GetUser().Messenger.OnNewFriendship(userRoom.UserId);
            }

            if (!session.GetUser().Messenger.FriendshipExists(user.GetUser().Id))
            {
                session.GetUser().Messenger.OnNewFriendship(user.GetUser().Id);
            }
        }
    }
}
