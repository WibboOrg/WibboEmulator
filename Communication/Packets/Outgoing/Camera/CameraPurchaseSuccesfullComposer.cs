namespace WibboEmulator.Communication.Packets.Outgoing.Camera;

internal sealed class CameraPurchaseSuccesfullComposer : ServerPacket
{
    public CameraPurchaseSuccesfullComposer()
        : base(ServerPacketHeader.CAMERA_PURCHASE_OK)
    {
    }
}