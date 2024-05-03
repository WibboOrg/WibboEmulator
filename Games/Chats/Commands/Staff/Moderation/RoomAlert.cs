namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomAlert : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var message = CommandManager.MergeParams(parameters, 1);
        foreach (var roomUser in room.RoomUserManager.RoomUsers)
        {
            if (roomUser == null || roomUser.Client == null || session.User.Id == roomUser.UserId)
            {
                continue;
            }

            roomUser.Client.SendNotification(message);
        }
    }
}
