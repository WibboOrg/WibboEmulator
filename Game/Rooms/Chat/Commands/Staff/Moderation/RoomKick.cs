using Butterfly.Game.GameClients;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class RoomKick : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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

            room.SetTimeout(async () =>
            {
                List<RoomUser> userKick = new List<RoomUser>();
                foreach (RoomUser user in currentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (user != null && !user.IsBot && !user.GetClient().GetHabbo().HasFuse("fuse_mod") && user.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        userKick.Add(user);
                    }
                }
                
                foreach (RoomUser user in userKick)
                {
                    user.AllowMoveTo = false;
                    user.IsWalking = true;
                    user.AllowOverride = true;
                    user.GoalX = Room.GetGameMap().Model.DoorX;
                    user.GoalY = Room.GetGameMap().Model.DoorY;
                }

                await Task.Delay(3000);
                
                foreach (RoomUser user in userKick)
                {
                    if (MessageAlert.Length > 0)
                    {
                        user.GetClient().SendNotification(MessageAlert);
                    }

                    currentRoom.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                }
            });
        }
    }
}