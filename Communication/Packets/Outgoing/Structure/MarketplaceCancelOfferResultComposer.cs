namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class MarketplaceCancelOfferResultComposer : ServerPacket
    {
        public MarketplaceCancelOfferResultComposer(int OfferId, bool Success)
            : base(ServerPacketHeader.MARKETPLACE_CANCEL_SALE)
        {
            this.WriteInteger(OfferId);
            this.WriteBoolean(Success);
        }
    }
}
