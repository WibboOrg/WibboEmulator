using System;
using WibboEmulator.Games.Chats.Commands;
using WibboEmulator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class SuperPush : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
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

        if (targetRoomUser.Client.User.Id == session.User.Id)
        {
            return;
        }

        if (targetRoomUser.Client.User.PremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        if (Math.Abs(targetRoomUser.X - userRoom.X) <= 1 && Math.Abs(targetRoomUser.Y - userRoom.Y) <= 1)
        {
            int pushX = targetRoomUser.X - userRoom.X;
            int pushY = targetRoomUser.Y - userRoom.Y;

            if (pushX == 0 && pushY == 1)
            {
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 4);
            }
            else if (pushX == 0 && pushY == -1)
            {
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 4);
            }
            else if (pushX == 1 && pushY == 0)
            {
                targetRoomUser.MoveTo(targetRoomUser.X + 4, targetRoomUser.Y);
            }
            else if (pushX == -1 && pushY == 0)
            {
                targetRoomUser.MoveTo(targetRoomUser.X - 4, targetRoomUser.Y);
            }

            userRoom.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.superpush.chat.success", session.Langue), targetName), 0, false);
        }
        else
        {
            userRoom.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.superpush.fail", session.Langue), targetName));
        }
    }
}
