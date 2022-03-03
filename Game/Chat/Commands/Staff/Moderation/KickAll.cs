using Butterfly.Game.Clients;
using System.Collections.Generic;
using System.Linq;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class KickAll : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            List<RoomUser> local_1 = new List<RoomUser>();
            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (!user.IsBot && !user.GetClient().GetUser().HasFuse("fuse_no_kick") && Session.GetUser().Id != user.GetClient().GetUser().Id)
                {
                    user.GetClient().SendNotification("Tu as été exclu de cet appart.");

                    local_1.Add(user);
                }
            }
            foreach (RoomUser item_1 in local_1)
            {
                if (item_1 == null || item_1.GetClient() == null)
                {
                    continue;
                }

                room.GetRoomUserManager().RemoveUserFromRoom(item_1.GetClient(), true, false);
            }

        }
    }
}
