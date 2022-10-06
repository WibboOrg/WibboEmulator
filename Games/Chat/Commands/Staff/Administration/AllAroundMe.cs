namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllAroundMe : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (User == null)
        {
            return;
        }

        var Users = Room.GetRoomUserManager().GetRoomUsers();
        foreach (var U in Users.ToList())
        {
            if (U == null || session.GetUser().Id == U.UserId)
            {
                continue;
            }

            U.MoveTo(User.X, User.Y, true);
        }
    }
}
