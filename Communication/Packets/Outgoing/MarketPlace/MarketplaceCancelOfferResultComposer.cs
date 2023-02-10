namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;

internal sealed class MarketplaceCancelOfferResultComposer : ServerPacket
{
    public MarketplaceCancelOfferResultComposer(int offerId, bool success)
        : base(ServerPacketHeader.MARKETPLACE_CANCEL_SALE)
    {
        this.WriteInteger(offerId);
        this.WriteBoolean(success);
    }
}
