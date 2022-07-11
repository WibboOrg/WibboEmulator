using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
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
