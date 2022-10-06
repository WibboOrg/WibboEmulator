namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;

internal class MarketplaceMakeOfferResultComposer : ServerPacket
{
    public MarketplaceMakeOfferResultComposer(int success)
        : base(ServerPacketHeader.MARKETPLACE_ITEM_POSTED) => this.WriteInteger(success);
}
