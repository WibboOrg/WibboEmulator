namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomAlert : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var message = CommandManager.MergeParams(parameters, 1);
        foreach (var roomUser in room.GetRoomUserManager().GetRoomUsers())
        {
            if (roomUser == null || roomUser.GetClient() == null || session.GetUser().Id == roomUser.UserId)
            {
                continue;
            }

            roomUser.GetClient().SendNotification(message);
        }
    }
}
