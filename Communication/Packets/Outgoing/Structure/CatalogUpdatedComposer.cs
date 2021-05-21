namespace Butterfly.Communication.Packets.Outgoing.Structure
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
