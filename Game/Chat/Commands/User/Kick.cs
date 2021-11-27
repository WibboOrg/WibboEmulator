using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Kick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetUser == null || TargetUser.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().Rank <= TargetUser.GetHabbo().Rank)
            {
                return;
            }

            if (TargetUser.GetHabbo().CurrentRoomId < 1U)
            {
                return;
            }

            Room.GetRoomUserManager().RemoveUserFromRoom(TargetUser, true, true);
        }
    }
}
