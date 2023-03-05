namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (clientByUsername != null)
        {
            var badgeCode = parameters[2];
            clientByUsername.User.BadgeComponent.GiveBadge(badgeCode, true);
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
    }
}
