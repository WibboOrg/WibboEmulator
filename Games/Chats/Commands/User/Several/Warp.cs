namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Warp : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null)
        {
            return;
        }

        var roomUserByUserIdTarget = room.RoomUserManager.GetRoomUserByUserId(targetUser.User.Id);
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        room.SendPacket(RoomItemHandling.TeleportUser(roomUserByUserIdTarget, userRoom.Coordinate, 0, room.GameMap.SqAbsoluteHeight(userRoom.X, userRoom.Y)));
    }
}
