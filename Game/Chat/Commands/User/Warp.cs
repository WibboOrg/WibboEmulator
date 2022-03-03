using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Warp : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(TargetUser.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(TargetUser.GetUser().Id);
            if (roomUserByUserIdTarget == null)
            {
                return;
            }

            room.SendPacket(room.GetRoomItemHandler().TeleportUser(roomUserByUserIdTarget, UserRoom.Coordinate, 0, room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));
        }
    }
}
