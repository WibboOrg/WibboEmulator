namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Nuke : IChatCommand
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

        if (targetUser.Client.User.PremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        var timeSpan = DateTime.Now - session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.fun.timeout", session.Langue), timeSpan.TotalSeconds));
            return;
        }

        session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.nuke.chat", session.Langue), targetUser.GetUsername()), 27);
        targetUser.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.nuke.chat.target", session.Langue), userRoom.GetUsername()), 18);

        targetUser.ApplyEffect(108, true);
        targetUser.TimerResetEffect = 6;
    }
}
