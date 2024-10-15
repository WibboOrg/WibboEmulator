namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Tomato : IChatCommand
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

        var timeSpan = DateTime.Now - session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat(string.Format(LanguageManager.TryGetValue("cmd.fun.timeout", session.Language), 10 - (int)timeSpan.TotalSeconds));
            return;
        }

        session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat(string.Format(LanguageManager.TryGetValue("cmd.tomato.chat", session.Language), targetUser.Username), 3);
        targetUser.OnChat(string.Format(LanguageManager.TryGetValue("cmd.tomato.chat.target", session.Language), userRoom.Username), 3);

        targetUser.CarryItem(98);
    }
}
