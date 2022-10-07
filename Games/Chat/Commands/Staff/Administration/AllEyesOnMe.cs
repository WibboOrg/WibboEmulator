namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

internal class AllEyesOnMe : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var thisUser = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (thisUser == null)
        {
            return;
        }

        var users = room.GetRoomUserManager().GetRoomUsers();
        foreach (var u in users.ToList())
        {
            if (u == null || session.GetUser().Id == u.UserId)
            {
                continue;
            }

            u.SetRot(Rotation.Calculate(u.X, u.Y, thisUser.X, thisUser.Y), false);
        }
    }
}
