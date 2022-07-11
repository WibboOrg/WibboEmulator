using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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