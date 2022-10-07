namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllAroundMe : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var users = room.GetRoomUserManager().GetRoomUsers();
        foreach (var user in users.ToList())
        {
            if (user == null || session.GetUser().Id == user.UserId)
            {
                continue;
            }

            user.MoveTo(userRoom.X, userRoom.Y, true);
        }
    }
}
