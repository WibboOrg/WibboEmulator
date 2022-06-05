using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class GiveItem : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null || roomUserByUserId.CarryItemID <= 0 || roomUserByUserId.CarryTimer <= 0)
            {
                return;
            }

            RoomUser roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (roomUserByUserIdTarget == null)
            {
                return;
            }

            if (Math.Abs(roomUserByUserId.X - roomUserByUserIdTarget.X) >= 3 || Math.Abs(roomUserByUserId.Y - roomUserByUserIdTarget.Y) >= 3)
            {
                return;
            }

            roomUserByUserIdTarget.CarryItem(roomUserByUserId.CarryItemID);
            roomUserByUserId.CarryItem(0);

        }
    }
}
