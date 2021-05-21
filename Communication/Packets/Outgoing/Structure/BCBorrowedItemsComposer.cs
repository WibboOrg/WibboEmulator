namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class BCBorrowedItemsComposer : ServerPacket
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.CATALOG_MODE)
        {
            this.WriteInteger(0);
        }
    }
}
