namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal sealed class MarketplaceConfigurationComposer : ServerPacket
{
    public MarketplaceConfigurationComposer(bool enabled = true, int commission = 0, int credits = 0, int advertisements = 0, int minimumPrice = 1, int maximumPrice = 99999, int offerTime = 48, int displayTime = 7)
        : base(ServerPacketHeader.MARKETPLACE_CONFIG)
    {
        this.WriteBoolean(enabled);
        this.WriteInteger(commission);
        this.WriteInteger(credits);
        this.WriteInteger(advertisements);
        this.WriteInteger(minimumPrice);
        this.WriteInteger(maximumPrice);
        this.WriteInteger(offerTime);
        this.WriteInteger(displayTime);
    }
}
