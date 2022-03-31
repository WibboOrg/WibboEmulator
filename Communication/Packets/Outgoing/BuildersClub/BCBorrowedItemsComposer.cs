namespace Butterfly.Communication.Packets.Outgoing.BuildersClub
{
    internal class BCBorrowedItemsComposer : ServerPacket
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BUILDERS_CLUB_FURNI_COUNT)
        {
            this.WriteInteger(0);
        }
    }
}
