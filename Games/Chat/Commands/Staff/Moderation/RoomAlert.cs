namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomAlert : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var Message = CommandManager.MergeParams(parameters, 1);
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