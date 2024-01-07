namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
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

        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(targetName);
        if (clientByUsername == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        clientByUsername.User.BadgeComponent.GiveBadge(badgeCode);
    }
}
