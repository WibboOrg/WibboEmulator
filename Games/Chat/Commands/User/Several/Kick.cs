namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Kick : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser user, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);

        if (targetUser == null || targetUser.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().Rank <= targetUser.GetUser().Rank)
        {
            return;
        }

        room.
        RoomUserManager.RemoveUserFromRoom(targetUser, true, true);
    }
}
