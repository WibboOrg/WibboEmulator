namespace WibboEmulator.Communication.Packets.Outgoing.Camera;

internal class CameraURLComposer : ServerPacket
{
    public CameraURLComposer(string Url)
        : base(ServerPacketHeader.CAMERA_STORAGE_URL) => this.WriteString(Url);
}