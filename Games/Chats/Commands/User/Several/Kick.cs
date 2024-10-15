namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Kick : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser user, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);

        if (TargetUser == null || TargetUser.User == null)
        {
            return;
        }

        if (Session.User.Rank <= TargetUser.User.Rank)
        {
            return;
        }

        room.RoomUserManager.RemoveUserFromRoom(TargetUser, true, true);
    }
}
