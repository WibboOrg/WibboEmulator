namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Freeze : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = userRoom.Room.RoomUserManager.GetRoomUserByName(parameters[1]);
        if (TargetUser == null)
        {
            return;
        }

        TargetUser.Freeze = !TargetUser.Freeze;
        TargetUser.FreezeEndCounter = 0;
    }
}
