namespace Butterfly.Communication.Packets.Outgoing.Camera
{
    internal class CameraURLComposer : ServerPacket
    {
        public CameraURLComposer(string Url)
            : base(ServerPacketHeader.CAMERA_URL)
        {
            this.WriteString(Url);
        }
    }
}