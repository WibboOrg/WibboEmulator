using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Teleport : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            roomUserByUserId.TeleportEnabled = !roomUserByUserId.TeleportEnabled;
        }
    }
}
