namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Trigger : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Convert.ToString(parameters[1]));

        if (TargetRoomUser == null || TargetRoomUser.GetClient() == null || TargetRoomUser.GetClient().GetUser() == null)
        {
            return;
        }

        if (TargetRoomUser.GetClient().GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (!(Math.Abs(TargetRoomUser.X - UserRoom.X) >= 2 || Math.Abs(TargetRoomUser.Y - UserRoom.Y) >= 2))
        {
            Room.OnTriggerUser(TargetRoomUser, true);
            Room.OnTriggerUser(UserRoom, false);
        }
    }
}
