namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllAroundMe : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var users = room.RoomUserManager.GetRoomUsers();
        foreach (var user in users.ToList())
        {
            if (user == null || session.User.Id == user.UserId)
            {
                continue;
            }

            user.MoveTo(userRoom.X, userRoom.Y, true);
        }
    }
}
