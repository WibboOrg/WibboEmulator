namespace Butterfly.Communication.Packets.Outgoing.Structure
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