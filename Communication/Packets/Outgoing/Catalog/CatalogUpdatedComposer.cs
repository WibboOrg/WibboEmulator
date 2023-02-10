namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal sealed class CatalogUpdatedComposer : ServerPacket
{
    public CatalogUpdatedComposer()
        : base(ServerPacketHeader.CATALOG_PUBLISHED) => this.WriteBoolean(false);
}
