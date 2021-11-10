using Butterfly.Game.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class AllAroundMe : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
                return;

            List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser U in Users.ToList())
            {
                if (U == null || Session.GetHabbo().Id == U.UserId)
                    continue;

                U.MoveTo(User.X, User.Y, true);
                //U.MoveTo(User.X, User.Z, true);
                //U.UnlockWalking();
            }
        }
    }
}
