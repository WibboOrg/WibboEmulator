namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal sealed class VoucherRedeemErrorComposer : ServerPacket
{
    public VoucherRedeemErrorComposer(int type)
        : base(ServerPacketHeader.REDEEM_VOUCHER_ERROR) => this.WriteString(type.ToString());
}
