using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ErrorCode = 1;

            Session.SendPacket(new MarketplaceCanMakeOfferResultComposer(ErrorCode));
        }
    }
}