using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Camera
{
    internal class PublishPhotoEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {

        }
    }
}
