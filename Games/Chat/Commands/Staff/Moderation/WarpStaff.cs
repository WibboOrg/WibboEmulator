namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class WarpStaff : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (TargetUser == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(TargetUser.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(TargetUser.GetUser().Id);
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        room.SendPacket(room.GetRoomItemHandler().TeleportUser(roomUserByUserIdTarget, roomUserByUserId.Coordinate, 0, room.GetGameMap().SqAbsoluteHeight(roomUserByUserId.X, roomUserByUserId.Y)));
    }
}
