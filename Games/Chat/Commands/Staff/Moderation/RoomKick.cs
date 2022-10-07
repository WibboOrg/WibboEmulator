namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomKick : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var messageAlert = CommandManager.MergeParams(parameters, 1);
        if (session.Antipub(messageAlert, "<CMD>"))
        {
            return;
        }

        _ = room.RunTask(async () =>
        {
            var userKick = new List<RoomUser>();
            foreach (var user in room.GetRoomUserManager().GetUserList().ToList())
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
                user.GoalX = room.GetGameMap().Model.DoorX;
                user.GoalY = room.GetGameMap().Model.DoorY;
            }

            await Task.Delay(3000);

            foreach (var user in userKick)
            {
                if (messageAlert.Length > 0)
                {
                    user.GetClient().SendNotification(messageAlert);
                }

                room.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
            }
        });
    }
}
