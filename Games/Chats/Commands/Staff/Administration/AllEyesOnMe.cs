namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

internal sealed class AllEyesOnMe : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var thisUser = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (thisUser == null)
        {
            return;
        }

        var users = room.RoomUserManager.RoomUsers;
        foreach (var u in users.ToList())
        {
            if (u == null || Session.User.Id == u.UserId)
            {
                continue;
            }

            u.SetRot(Rotation.Calculate(u.X, u.Y, thisUser.X, thisUser.Y), false);
        }
    }
}
