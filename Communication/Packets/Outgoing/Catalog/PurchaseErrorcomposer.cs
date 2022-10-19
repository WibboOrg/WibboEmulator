namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal class PurchaseErrorComposer : ServerPacket
{
    public PurchaseErrorComposer(int code = -1)
        : base(ServerPacketHeader.CATALOG_PURCHASE_ERROR) => this.WriteInteger(code);
}
