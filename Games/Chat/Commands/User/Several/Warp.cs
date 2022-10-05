namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Warp : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);
        if (TargetUser == null)
        {
            return;
        }

        var roomUserByUserIdTarget = Room.GetRoomUserManager().GetRoomUserByUserId(TargetUser.GetUser().Id);
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        Room.SendPacket(Room.GetRoomItemHandler().TeleportUser(roomUserByUserIdTarget, UserRoom.Coordinate, 0, Room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));
    }
}
