namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;

internal sealed class CatalogOfferComposer : ServerPacket
{
    public CatalogOfferComposer(CatalogItem item, bool isPremium)
        : base(ServerPacketHeader.PRODUCT_OFFER) => CatalogItemUtility.GenerateOfferData(item, isPremium, this);
}