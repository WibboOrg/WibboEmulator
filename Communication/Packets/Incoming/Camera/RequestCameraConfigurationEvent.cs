using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RequestCameraConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CameraPriceComposer(0, 0, 0));
        }
    }
}
