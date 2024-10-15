namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Warp : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (TargetUser == null)
        {
            return;
        }

        var roomUserByUserIdTarget = room.RoomUserManager.GetRoomUserByUserId(TargetUser.User.Id);
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        room.SendPacket(RoomItemHandling.TeleportUser(roomUserByUserIdTarget, userRoom.Coordinate, 0, room.GameMap.SqAbsoluteHeight(userRoom.X, userRoom.Y)));
    }
}
