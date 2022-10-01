using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
{
    internal class PublishPhotoEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {

        }
    }
}
