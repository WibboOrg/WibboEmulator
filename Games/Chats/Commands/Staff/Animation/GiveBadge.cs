namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return;
        }

        var targetName = parameters[1];
        var badgeCode = parameters[2];

        var clientByUsername = GameClientManager.GetClientByUsername(targetName);
        if (clientByUsername == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
            return;
        }

        clientByUsername.User.BadgeComponent.GiveBadge(badgeCode);
    }
}
