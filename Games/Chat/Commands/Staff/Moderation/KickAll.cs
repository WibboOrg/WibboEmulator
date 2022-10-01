using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class KickAll : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser userRoom, string[] parameters)
        {
            List<RoomUser> roomUserList = new List<RoomUser>();
            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (!user.IsBot && !user.GetClient().GetUser().HasPermission("perm_no_kick") && session.GetUser().Id != user.GetClient().GetUser().Id)
                {
                    user.GetClient().SendNotification("Tu as été exclu de cet appart.");

                    roomUserList.Add(user);
                }
            }
            foreach (RoomUser user in roomUserList)
            {
                if (user == null || user.GetClient() == null)
                {
                    continue;
                }

                room.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
            }
        }
    }
}
