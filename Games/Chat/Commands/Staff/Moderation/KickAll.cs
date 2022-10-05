namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class KickAll : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var roomUserList = new List<RoomUser>();
        foreach (var user in room.GetRoomUserManager().GetUserList().ToList())
        {
            if (!user.IsBot && !user.GetClient().GetUser().HasPermission("perm_no_kick") && session.GetUser().Id != user.GetClient().GetUser().Id)
            {
                user.GetClient().SendNotification("Tu as été exclu de cet appart.");

                roomUserList.Add(user);
            }
        }
        foreach (var user in roomUserList)
        {
            if (user == null || user.GetClient() == null)
            {
                continue;
            }

            room.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
        }
    }
}
