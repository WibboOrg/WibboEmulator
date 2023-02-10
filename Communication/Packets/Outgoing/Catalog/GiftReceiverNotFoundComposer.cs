namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal sealed class GiftReceiverNotFoundComposer : ServerPacket
{
    public GiftReceiverNotFoundComposer()
        : base(ServerPacketHeader.GIFT_RECEIVER_NOT_FOUND)
    {
    }
}
