namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GiveItem : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        if (UserRoom.CarryItemID <= 0 || UserRoom.CarryTimer <= 0)
        {
            return;
        }

        var roomUserByUserIdTarget = Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        if (Math.Abs(UserRoom.X - roomUserByUserIdTarget.X) >= 3 || Math.Abs(UserRoom.Y - roomUserByUserIdTarget.Y) >= 3)
        {
            return;
        }

        roomUserByUserIdTarget.CarryItem(UserRoom.CarryItemID);
        UserRoom.CarryItem(0);
    }
}
