namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Freeze : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var TargetUser = UserRoom.Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
        if (TargetUser == null)
        {
            return;
        }

        TargetUser.Freeze = !TargetUser.Freeze;
        TargetUser.FreezeEndCounter = 0;
    }
}
