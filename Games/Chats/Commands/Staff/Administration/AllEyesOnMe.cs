namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

internal class AllEyesOnMe : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var thisUser = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (thisUser == null)
        {
            return;
        }

        var users = room.RoomUserManager.GetRoomUsers();
        foreach (var u in users.ToList())
        {
            if (u == null || session.User.Id == u.UserId)
            {
                continue;
            }

            u.SetRot(Rotation.Calculate(u.X, u.Y, thisUser.X, thisUser.Y), false);
        }
    }
}
