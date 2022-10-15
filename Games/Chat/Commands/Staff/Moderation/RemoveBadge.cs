namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RemoveBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser != null && targetUser.User != null)
        {
            targetUser.User.BadgeComponent.RemoveBadge(parameters[2]);
            targetUser.SendPacket(new BadgesComposer(targetUser.User.BadgeComponent.BadgeList));
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
    }
}
