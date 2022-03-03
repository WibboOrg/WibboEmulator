using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class TeleportStaff : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            RoomUser roomUserByUserId = currentRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            roomUserByUserId.TeleportEnabled = !roomUserByUserId.TeleportEnabled;
        }
    }
}
