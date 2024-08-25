namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var targetUser = GameClientManager.GetClientByUsername(parameters[1]);
        var badgeCode = parameters[2];

        if (targetUser == null || targetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
            return;
        }

        if (!targetUser.User.BadgeComponent.HasBadge(badgeCode))
        {
            session.SendHugeNotification(LanguageManager.TryGetValue("notif.badge.removed.error", session.Language));
            return;
        }

        targetUser.User.BadgeComponent.RemoveBadge(parameters[2]);
    }
}
