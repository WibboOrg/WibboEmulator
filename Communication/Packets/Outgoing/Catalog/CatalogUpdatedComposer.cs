namespace Butterfly.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogUpdatedComposer : ServerPacket
    {
        public CatalogUpdatedComposer()
            : base(ServerPacketHeader.CATALOG_UPDATED)
        {
            this.WriteBoolean(false);
        }
    }
}