using Butterfly.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class KickAll : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomIvokedItems.RoomKick kick = new RoomIvokedItems.RoomKick("Tu as été exclu de cet appart.", Session.GetHabbo().Id);
            List<RoomUser> local_1 = new List<RoomUser>();
            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (!user.IsBot && !user.GetClient().GetHabbo().HasFuse("fuse_no_kick") && kick.SaufId != user.GetClient().GetHabbo().Id)
                {
                    if (kick.Alert.Length > 0)
                    {
                        user.GetClient().SendNotification(kick.Alert);
                    }

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
