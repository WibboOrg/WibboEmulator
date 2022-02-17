using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ErrorCode = 1;

            Session.SendPacket(new MarketplaceCanMakeOfferResultComposer(ErrorCode));
        }
    }
}