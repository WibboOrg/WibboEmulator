using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Camera
{
    internal class PublishPhotoEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {

        }
    }
}
