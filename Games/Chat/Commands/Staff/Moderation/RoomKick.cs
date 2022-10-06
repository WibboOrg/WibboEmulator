namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomKick : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var MessageAlert = CommandManager.MergeParams(parameters, 1);
        if (session.Antipub(MessageAlert, "<CMD>"))
        {
            return;
        }

        _ = Room.RunTask(async () =>
        {
            var userKick = new List<RoomUser>();
            foreach (var user in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user != null && !user.IsBot && !user.GetClient().GetUser().HasPermission("perm_mod") && user.GetClient().GetUser().Id != session.GetUser().Id)
                {
                    userKick.Add(user);
                }
            }

            foreach (var user in userKick)
            {
                user.AllowMoveTo = false;
                user.IsWalking = true;
                user.AllowOverride = true;
                user.GoalX = Room.GetGameMap().Model.DoorX;
                user.GoalY = Room.GetGameMap().Model.DoorY;
            }

            await Task.Delay(3000);

            foreach (var user in userKick)
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