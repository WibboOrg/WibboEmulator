using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Sit : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = Session.GetUser().CurrentRoom;
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.Statusses.ContainsKey("sit") || roomUserByUserId.Statusses.ContainsKey("lay"))
            {
                return;
            }

            if (roomUserByUserId.RotBody % 2 == 0)
            {
                if (UserRoom.IsTransf)
                {
                    roomUserByUserId.SetStatus("sit", "");
                }
                else
                {
                    roomUserByUserId.SetStatus("sit", "0.5");
                }

                roomUserByUserId.IsSit = true;
                roomUserByUserId.UpdateNeeded = true;
            }

        }
    }
}
