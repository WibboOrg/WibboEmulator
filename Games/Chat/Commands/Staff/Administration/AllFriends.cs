using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class AllFriends : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (GameClient User in WibboEnvironment.GetGame().GetGameClientManager().GetClients)
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

                if (!Session.GetUser().GetMessenger().FriendshipExists(User.GetUser().Id))
                {
                    Session.GetUser().GetMessenger().OnNewFriendship(User.GetUser().Id);
                }
            }
        }
    }
}
