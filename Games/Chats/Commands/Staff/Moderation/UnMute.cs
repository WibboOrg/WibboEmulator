namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class UnMute : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        if (string.IsNullOrEmpty(username))
        {
            return;
        }

        var targetUser = GameClientManager.GetClientByUsername(username);
        if (targetUser == null || targetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
        }
        else
        {
            targetUser.User.SpamProtectionTime = 10;
            targetUser.User.SpamEnable = true;
        }
    }
}
