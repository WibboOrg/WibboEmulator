namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RemoveBadge : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (TargetUser != null && TargetUser.GetUser() != null)
        {
            TargetUser.GetUser().GetBadgeComponent().RemoveBadge(parameters[2]);
            TargetUser.SendPacket(new BadgesComposer(TargetUser.GetUser().GetBadgeComponent().BadgeList));
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
    }
}
