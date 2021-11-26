using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RemoveBadge : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername != null && clientByUsername.GetHabbo() != null)
            {
                clientByUsername.GetHabbo().GetBadgeComponent().RemoveBadge(Params[2]);
                clientByUsername.SendPacket(new BadgesComposer(clientByUsername.GetHabbo().GetBadgeComponent().BadgeList));
            }
            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
        }
    }
}
