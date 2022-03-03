using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetBadgesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            Session.SendPacket(new BadgesComposer(Session.GetUser().GetBadgeComponent().BadgeList));
        }
    }
}