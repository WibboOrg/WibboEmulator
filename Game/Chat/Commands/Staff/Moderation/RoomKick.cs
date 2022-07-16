using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class RoomKick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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

                    currentRoom.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                }
            });
        }
    }
}