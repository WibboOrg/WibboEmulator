using Butterfly.HabboHotel.GameClients;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class RoomKick : IChatCommand
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

            string MessageAlert = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(MessageAlert, "<CMD>"))
            {
                return;
            }

            foreach (RoomUser User in currentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (User != null && !User.IsBot && !User.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                {
                    User.AllowMoveTo = false;
                    User.IsWalking = true;
                    User.AllowOverride = true;
                    User.GoalX = Room.GetGameMap().Model.DoorX;
                    User.GoalY = Room.GetGameMap().Model.DoorY;
                }
            }


            room.SetTimeout(2500, () =>
            {
                foreach (RoomUser User in currentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (User != null && !User.IsBot && !User.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                    {
                        if (MessageAlert.Length > 0)
                        {
                            User.GetClient().SendNotification(MessageAlert);
                        }

                        currentRoom.GetRoomUserManager().RemoveUserFromRoom(User.GetClient(), true, false);
                    }
                }
            });
        }
    }
}
