using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Kick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                return;
            }

            if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                return;
            }

            if (TargetUser.GetUser().CurrentRoomId < 1U)
            {
                return;
            }

            Room.GetRoomUserManager().RemoveUserFromRoom(TargetUser, true, true);
        }
    }
}
