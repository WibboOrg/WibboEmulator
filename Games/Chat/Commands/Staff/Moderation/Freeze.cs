namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Freeze : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = UserRoom.Room.GetRoomUserManager().GetRoomUserByName(parameters[1]);
        if (TargetUser == null)
        {
            return;
        }

        TargetUser.Freeze = !TargetUser.Freeze;
        TargetUser.FreezeEndCounter = 0;
    }
}
