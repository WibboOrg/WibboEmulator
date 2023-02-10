namespace WibboEmulator.Communication.Packets.Outgoing.Camera;

internal sealed class CameraURLComposer : ServerPacket
{
    public CameraURLComposer(string url)
        : base(ServerPacketHeader.CAMERA_STORAGE_URL) => this.WriteString(url);
}
