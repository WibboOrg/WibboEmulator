namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Warp : IChatCommand
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

        var roomUserByUserIdTarget = Room.GetRoomUserManager().GetRoomUserByUserId(TargetUser.GetUser().Id);
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        Room.SendPacket(Room.GetRoomItemHandler().TeleportUser(roomUserByUserIdTarget, UserRoom.Coordinate, 0, Room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));
    }
}
