namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class AllAroundMe : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var users = room.RoomUserManager.RoomUsers;
        foreach (var user in users.ToList())
        {
            if (user == null || Session.User.Id == user.UserId)
            {
                continue;
            }

            user.MoveTo(userRoom.X, userRoom.Y, true);
        }
    }
}
