namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Laser : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = room.RoomUserManager.GetRoomUserByName(parameters[1]);
        if (TargetUser == null || TargetUser.Client == null || TargetUser.Client.User == null)
        {
            return;
        }

        if (TargetUser.Client.User.Id == Session.User.Id)
        {
            return;
        }

        if (TargetUser.Client.User.HasPremiumProtect && !Session.User.HasPermission("mod"))
        {
            Session.SendWhisper(LanguageManager.TryGetValue("premium.notallowed", Session.Language));
            return;
        }

        if (Math.Abs(TargetUser.X - userRoom.X) >= 2 || Math.Abs(TargetUser.Y - userRoom.Y) >= 2)
        {
            return;
        }

        var timeSpan = DateTime.Now - Session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.fun.timeout", Session.Language), 10 - (int)timeSpan.TotalSeconds));
            return;
        }

        Session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.laser.chat", Session.Language), TargetUser.Username), 22);
        TargetUser.OnChat(string.Format(LanguageManager.TryGetValue("cmd.laser.chat.target", Session.Language), userRoom.Username), 18);

        userRoom.ApplyEffect(196, true);
        userRoom.TimerResetEffect = 6;

        TargetUser.ApplyEffect(93, true);
        TargetUser.TimerResetEffect = 6;
    }
}
