namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal class GiftReceiverNotFoundComposer : ServerPacket
{
    public GiftReceiverNotFoundComposer()
        : base(ServerPacketHeader.GIFT_RECEIVER_NOT_FOUND)
    {
    }
}
