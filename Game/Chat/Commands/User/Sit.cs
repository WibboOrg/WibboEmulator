using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Sit : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            if (roomUserByHabbo.Statusses.ContainsKey("sit") || roomUserByHabbo.Statusses.ContainsKey("lay"))
            {
                return;
            }

            if (roomUserByHabbo.RotBody % 2 == 0)
            {
                if (UserRoom.transformation)
                {
                    roomUserByHabbo.SetStatus("sit", "");
                }
                else
                {
                    roomUserByHabbo.SetStatus("sit", "0.5");
                }

                roomUserByHabbo.IsSit = true;
                roomUserByHabbo.UpdateNeeded = true;
            }

        }
    }
}