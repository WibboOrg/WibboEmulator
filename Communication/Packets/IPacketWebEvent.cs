using Butterfly.Communication.Packets.Incoming;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets
{
    public interface IPacketWebEvent
    {
        void Parse(WebClient session, ClientPacket packet);
    }
}
