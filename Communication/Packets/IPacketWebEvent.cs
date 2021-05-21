using Butterfly.Communication.Packets.Incoming;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets
{
    public interface IPacketWebEvent
    {
        void Parse(WebClient session, ClientPacket packet);
    }
}
