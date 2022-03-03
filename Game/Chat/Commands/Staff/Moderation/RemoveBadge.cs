using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RemoveBadge : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser != null && TargetUser.GetUser() != null)
            {
                TargetUser.GetUser().GetBadgeComponent().RemoveBadge(Params[2]);
                TargetUser.SendPacket(new BadgesComposer(TargetUser.GetUser().GetBadgeComponent().BadgeList));
            }
            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}
