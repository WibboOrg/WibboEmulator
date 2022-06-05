using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class AllFriends : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (Client User in WibboEnvironment.GetGame().GetClientManager().GetClients)
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
