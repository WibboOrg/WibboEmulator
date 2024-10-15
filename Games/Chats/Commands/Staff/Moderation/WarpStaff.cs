namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class WarpStaff : IChatCommand
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

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        room.SendPacket(RoomItemHandling.TeleportUser(roomUserByUserIdTarget, roomUserByUserId.Coordinate, 0, room.GameMap.SqAbsoluteHeight(roomUserByUserId.X, roomUserByUserId.Y)));
    }
}
