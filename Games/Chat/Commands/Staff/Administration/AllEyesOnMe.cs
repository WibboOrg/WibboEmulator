namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

internal class AllEyesOnMe : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var ThisUser = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (ThisUser == null)
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

            U.SetRot(Rotation.Calculate(U.X, U.Y, ThisUser.X, ThisUser.Y), false);
        }
    }
}
