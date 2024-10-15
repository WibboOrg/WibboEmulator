namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Push : IChatCommand
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

        if (targetRoomUser.Client.User.HasPremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", session.Language));
            return;
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2) || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2)
        {
            if (targetRoomUser.SetX - 1 == room.GameMap.Model.DoorX)
            {
                return;
            }
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2 || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2))
        {
            if (userRoom.RotBody == 4)
            { targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 1); }

            if (userRoom.RotBody == 0)
            { targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 1); }

            if (userRoom.RotBody == 6)
            { targetRoomUser.MoveTo(targetRoomUser.X - 1, targetRoomUser.Y); }

            if (userRoom.RotBody == 2)
            { targetRoomUser.MoveTo(targetRoomUser.X + 1, targetRoomUser.Y); }

            if (userRoom.RotBody == 3)
            {
                targetRoomUser.MoveTo(targetRoomUser.X + 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 1);
            }

            if (userRoom.RotBody == 1)
            {
                targetRoomUser.MoveTo(targetRoomUser.X + 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 1);
            }

            if (userRoom.RotBody == 7)
            {
                targetRoomUser.MoveTo(targetRoomUser.X - 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 1);
            }

            if (userRoom.RotBody == 5)
            {
                targetRoomUser.MoveTo(targetRoomUser.X - 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 1);
            }

            userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.push.chat.success", session.Language), targetName), 0, false);
        }
        else
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.push.fail", session.Language), targetName));
        }
    }
}
