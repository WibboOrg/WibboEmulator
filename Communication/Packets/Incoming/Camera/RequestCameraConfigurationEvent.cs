using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Camera
{
    internal class RequestCameraConfigurationEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new CameraPriceComposer(0, 0, 0));
        }
    }
}
