using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class RoomKick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string MessageAlert = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(MessageAlert, "<CMD>"))
            {
                return;
            }

            Room.SetTimeout(async () =>
            {
                List<RoomUser> userKick = new List<RoomUser>();
                foreach (RoomUser user in Room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (user != null && !user.IsBot && !user.GetClient().GetUser().HasPermission("perm_mod") && user.GetClient().GetUser().Id != Session.GetUser().Id)
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

                    Room.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                }
            });
        }
    }
}