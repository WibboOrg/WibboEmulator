namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomAlert : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var Message = CommandManager.MergeParams(Params, 1);
        foreach (var RoomUser in Room.GetRoomUserManager().GetRoomUsers())
        {
            if (RoomUser == null || RoomUser.GetClient() == null || session.GetUser().Id == RoomUser.UserId)
            {
                continue;
            }

            RoomUser.GetClient().SendNotification(Message);
        }
    }
}