using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
{
    internal class RequestCameraConfigurationEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet) => Session.SendPacket(new CameraPriceComposer(0, 0, 0));
    }
}
