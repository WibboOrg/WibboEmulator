using Butterfly.Communication.Packets.Outgoing.MarketPlace;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ErrorCode = 1;

            Session.SendPacket(new MarketplaceCanMakeOfferResultComposer(ErrorCode));
        }
    }
}