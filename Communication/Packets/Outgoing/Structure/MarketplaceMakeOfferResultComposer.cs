namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class MarketplaceMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceMakeOfferResultComposer(int Success)
            : base(ServerPacketHeader.MARKETPLACE_ITEM_POSTED)
        {
            this.WriteInteger(Success);
        }
    }
}
