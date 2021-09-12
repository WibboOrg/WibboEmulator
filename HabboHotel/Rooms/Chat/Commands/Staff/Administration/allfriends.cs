using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class AllFriends : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            foreach (GameClient User in ButterflyEnvironment.GetGame().GetClientManager().GetClients)
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
