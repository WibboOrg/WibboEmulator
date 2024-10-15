namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Slime : IChatCommand
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

        var timeSpan = DateTime.Now - Session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.fun.timeout", Session.Language), 10 - (int)timeSpan.TotalSeconds));
            return;
        }

        Session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.slime.chat", Session.Language), TargetUser.Username));
        TargetUser.OnChat(string.Format(LanguageManager.TryGetValue("cmd.slime.chat.target", Session.Language), userRoom.Username));

        TargetUser.ApplyEffect(169, true);
        TargetUser.TimerResetEffect = 6;
    }
}
