using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Kick : IChatCommand
    {
        public void Execute(GameClient session, Room room, RoomUser user, string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            GameClient TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(parameters[1]);

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
}
