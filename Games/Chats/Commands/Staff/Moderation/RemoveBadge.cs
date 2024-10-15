namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveBadge : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);
        var badgeCode = parameters[2];

        if (TargetUser == null || TargetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
            return;
        }

        if (!TargetUser.User.BadgeComponent.HasBadge(badgeCode))
        {
            Session.SendHugeNotification(LanguageManager.TryGetValue("notif.badge.removed.error", Session.Language));
            return;
        }

        TargetUser.User.BadgeComponent.RemoveBadge(parameters[2]);
    }
}
