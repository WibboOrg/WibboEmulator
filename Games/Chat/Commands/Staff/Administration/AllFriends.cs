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

            if (user.GetUser().GetMessenger() == null)
            {
                continue;
            }

            if (!user.GetUser().GetMessenger().FriendshipExists(userRoom.UserId))
            {
                user.GetUser().GetMessenger().OnNewFriendship(userRoom.UserId);
            }

            if (!session.GetUser().GetMessenger().FriendshipExists(user.GetUser().Id))
            {
                session.GetUser().GetMessenger().OnNewFriendship(user.GetUser().Id);
            }
        }
    }
}
