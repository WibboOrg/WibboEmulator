using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class RemoveBadge : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser != null && TargetUser.GetUser() != null)
            {
                TargetUser.GetUser().GetBadgeComponent().RemoveBadge(Params[2]);
                TargetUser.SendPacket(new BadgesComposer(TargetUser.GetUser().GetBadgeComponent().BadgeList));
            }
            else
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}
