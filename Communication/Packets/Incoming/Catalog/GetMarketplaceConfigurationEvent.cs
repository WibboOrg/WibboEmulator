using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new MarketplaceConfigurationComposer());
        }
    }
}
