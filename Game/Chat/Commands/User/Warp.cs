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

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(TargetUser.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(TargetUser.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            RoomUser roomUserByHabbo2 = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo2 == null)
            {
                return;
            }

            room.SendPacket(room.GetRoomItemHandler().TeleportUser(roomUserByHabbo, roomUserByHabbo2.Coordinate, 0, room.GetGameMap().SqAbsoluteHeight(roomUserByHabbo2.X, roomUserByHabbo2.Y)));
            //room.GetRoomUserManager().UpdateUserStatus(roomUserByHabbo, false);

        }
    }
}
