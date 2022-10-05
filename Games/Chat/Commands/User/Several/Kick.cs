namespace WibboEmulator.Games.Chat.Commands.Cmd;
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

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);

        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().Rank <= TargetUser.GetUser().Rank)
        {
            return;
        }

        room.GetRoomUserManager().RemoveUserFromRoom(TargetUser, true, true);
    }
}
