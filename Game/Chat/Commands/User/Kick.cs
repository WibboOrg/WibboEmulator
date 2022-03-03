using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Kick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

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
