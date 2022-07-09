using Wibbo.Communication.Packets.Outgoing.MarketPlace;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Marketplace
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