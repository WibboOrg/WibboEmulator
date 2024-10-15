namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class UnMute : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
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

        var TargetUser = GameClientManager.GetClientByUsername(username);
        if (TargetUser == null || TargetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
        }
        else
        {
            TargetUser.User.SpamProtectionTime = 10;
            TargetUser.User.SpamEnable = true;
        }
    }
}
