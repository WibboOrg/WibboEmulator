using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Freeze : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            RoomUser TargetUser = UserRoom.Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (TargetUser == null)
            {
                return;
            }

            TargetUser.Freeze = !TargetUser.Freeze;
            TargetUser.FreezeEndCounter = 0;


        }
    }
}
