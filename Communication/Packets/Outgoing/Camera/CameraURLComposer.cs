namespace WibboEmulator.Communication.Packets.Outgoing.Camera;

internal class CameraURLComposer : ServerPacket
{
    public CameraURLComposer(string url)
        : base(ServerPacketHeader.CAMERA_STORAGE_URL) => this.WriteString(url);
}
