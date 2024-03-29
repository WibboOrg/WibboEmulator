namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;

internal sealed class MarketplaceCanMakeOfferResultComposer : ServerPacket
{
    public MarketplaceCanMakeOfferResultComposer(int result)
        : base(ServerPacketHeader.MARKETPLACE_SELL_ITEM)
    {
        this.WriteInteger(result);
        this.WriteInteger(0);
    }
}
