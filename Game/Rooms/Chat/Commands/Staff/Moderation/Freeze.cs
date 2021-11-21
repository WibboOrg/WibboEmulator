using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Freeze : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom.Room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (roomUserByHabbo == null)
            {
                return;
            }

            roomUserByHabbo.Freeze = !roomUserByHabbo.Freeze;
            roomUserByHabbo.FreezeEndCounter = 0;


        }
    }
}
