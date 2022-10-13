namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Trigger : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var targetRoomUser = room.RoomUserManager.GetRoomUserByName(Convert.ToString(parameters[1]));

        if (targetRoomUser == null || targetRoomUser.Client == null || targetRoomUser.Client.GetUser() == null)
        {
            return;
        }

        if (targetRoomUser.Client.GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2 || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2))
        {
            room.OnTriggerUser(targetRoomUser, true);
            room.OnTriggerUser(userRoom, false);
        }
    }
}
