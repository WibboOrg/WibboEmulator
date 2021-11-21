using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class AllFriends : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (Client User in ButterflyEnvironment.GetGame().GetClientManager().GetClients)
            {
                if (User == null)
                {
                    continue;
                }

                if (User.GetHabbo() == null)
                {
                    continue;
                }

                if (User.GetHabbo().GetMessenger() == null)
                {
                    continue;
                }

                if (!User.GetHabbo().GetMessenger().FriendshipExists(UserRoom.HabboId))
                {
                    User.GetHabbo().GetMessenger().OnNewFriendship(UserRoom.HabboId);
                }

                if (!Session.GetHabbo().GetMessenger().FriendshipExists(User.GetHabbo().Id))
                {
                    Session.GetHabbo().GetMessenger().OnNewFriendship(User.GetHabbo().Id);
                }
            }
        }
    }
}
