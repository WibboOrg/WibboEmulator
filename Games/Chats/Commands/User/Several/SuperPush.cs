using WibboEmulator.Games.Chats.Commands;
using WibboEmulator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Core.Language;

internal sealed class SuperPush : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (!room.PushPullAllowed)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var targetName = parameters[1];

        var targetRoomUser = room.RoomUserManager.GetRoomUserByName(targetName);

        if (targetRoomUser == null)
        {
            return;
        }

        if (targetRoomUser.Client.User.Id == Session.User.Id)
        {
            return;
        }

        if (targetRoomUser.Client.User.HasPremiumProtect && !Session.User.HasPermission("mod"))
        {
            Session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", Session.Language));
            return;
        }

        if (Math.Abs(targetRoomUser.X - userRoom.X) <= 1 && Math.Abs(targetRoomUser.Y - userRoom.Y) <= 1)
        {
            var pushX = targetRoomUser.X - userRoom.X;
            var pushY = targetRoomUser.Y - userRoom.Y;

            if (pushX == 0 && pushY == 1)
            {
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 4, true);
            }
            else if (pushX == 0 && pushY == -1)
            {
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 4, true);
            }
            else if (pushX == 1 && pushY == 0)
            {
                targetRoomUser.MoveTo(targetRoomUser.X + 4, targetRoomUser.Y, true);
            }
            else if (pushX == -1 && pushY == 0)
            {
                targetRoomUser.MoveTo(targetRoomUser.X - 4, targetRoomUser.Y, true);
            }

            userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.superpush.chat.success", Session.Language), targetName), 0, false);
        }
        else
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.superpush.fail", Session.Language), targetName));
        }
    }
}
