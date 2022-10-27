namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class KickAll : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var roomUserList = new List<RoomUser>();
        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (!user.IsBot && !user.Client.User.HasPermission("perm_no_kick") && session.User.Id != user.Client.User.Id)
            {
                user.Client.SendNotification("Tu as été exclu de cet appart.");

                roomUserList.Add(user);
            }
        }
        foreach (var user in roomUserList)
        {
            if (user == null || user.Client == null)
            {
                continue;
            }

            room.RoomUserManager.RemoveUserFromRoom(user.Client, true, false);
        }
    }
}
