using Butterfly.Communication.Packets.Incoming;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(Client session, ClientPacket packet);
    }
}
