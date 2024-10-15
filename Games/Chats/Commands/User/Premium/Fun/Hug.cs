namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Hug : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = room.RoomUserManager.GetRoomUserByName(parameters[1]);
        if (targetUser == null || targetUser.Client == null || targetUser.Client.User == null)
        {
            return;
        }

        if (targetUser.Client.User.Id == session.User.Id)
        {
            return;
        }

        if (targetUser.Client.User.HasPremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", session.Language));
            return;
        }

        if (Math.Abs(targetUser.X - userRoom.X) >= 2 || Math.Abs(targetUser.Y - userRoom.Y) >= 2)
        {
            return;
        }

        var timeSpan = DateTime.Now - session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.fun.timeout", session.Language), 10 - (int)timeSpan.TotalSeconds));
            return;
        }

        session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.hug.chat", session.Language), targetUser.Username), 16);
        targetUser.OnChat(string.Format(LanguageManager.TryGetValue("cmd.hug.chat.target", session.Language), userRoom.Username), 16);

        userRoom.ApplyEffect(9, true);
        userRoom.TimerResetEffect = 6;

        targetUser.ApplyEffect(9, true);
        targetUser.TimerResetEffect = 6;
    }
}
