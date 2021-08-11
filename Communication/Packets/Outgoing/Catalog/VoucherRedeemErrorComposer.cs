namespace Butterfly.Communication.Packets.Outgoing.Catalog
{
    internal class VoucherRedeemErrorComposer : ServerPacket
    {
        public VoucherRedeemErrorComposer(int Type)
            : base(ServerPacketHeader.REDEEM_VOUCHER_ERROR)
        {
            this.WriteString(Type.ToString());
        }
    }
}
