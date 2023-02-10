namespace WibboEmulator.Communication.Packets.Outgoing.Camera;

internal sealed class CameraPriceComposer : ServerPacket
{
    public CameraPriceComposer(int credits, int duckets, int publishDuckets)
        : base(ServerPacketHeader.INIT_CAMERA)
    {
        this.WriteInteger(credits);
        this.WriteInteger(duckets);
        this.WriteInteger(publishDuckets);
    }
}
