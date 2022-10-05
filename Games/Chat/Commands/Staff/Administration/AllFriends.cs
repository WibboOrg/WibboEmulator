namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllFriends : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        foreach (var User in WibboEnvironment.GetGame().GetGameClientManager().GetClients)
        {
            if (User == null)
            {
                continue;
            }

            if (User.GetUser() == null)
            {
                continue;
            }

            if (User.GetUser().GetMessenger() == null)
            {
                continue;
            }

            if (!User.GetUser().GetMessenger().FriendshipExists(UserRoom.UserId))
            {
                User.GetUser().GetMessenger().OnNewFriendship(UserRoom.UserId);
            }

            if (!session.GetUser().GetMessenger().FriendshipExists(User.GetUser().Id))
            {
                session.GetUser().GetMessenger().OnNewFriendship(User.GetUser().Id);
            }
        }
    }
}
