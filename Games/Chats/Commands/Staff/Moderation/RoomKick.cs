namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomKick : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var messageAlert = CommandManager.MergeParams(parameters, 1);
        if (session.User.Antipub(messageAlert, "<CMD>", room.Id))
        {
            return;
        }

        _ = room.RunTask(async () =>
        {
            var userKick = new List<RoomUser>();
            foreach (var user in room.RoomUserManager.GetUserList().ToList())
            {
                if (user != null && user.Client != null && !user.Client.User.HasPermission("mod") && user.Client.User.Id != session.User.Id)
                {
                    userKick.Add(user);
                }
            }

            foreach (var user in userKick)
            {
                user.AllowMoveTo = false;
                user.IsWalking = true;
                user.AllowOverride = true;
                user.GoalX = room.GameMap.Model.DoorX;
                user.GoalY = room.GameMap.Model.DoorY;
            }

            await Task.Delay(3000);

            foreach (var user in userKick)
            {
                if (user == null || user.Client == null || user.RoomId != room.Id)
                {
                    continue;
                }

                if (messageAlert.Length > 0)
                {
                    user.Client.SendNotification(messageAlert);
                }

                room.RoomUserManager.RemoveUserFromRoom(user.Client, true, false);

                if (user.Client.User.HomeRoom > 0)
                {
                    if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoomData(user.Client.User.HomeRoom, out var roomHome))
                    {
                        user.Client.SendPacket(new GetGuestRoomResultComposer(user.Client, roomHome, false, true));
                    }
                }
            }
        });
    }
}
