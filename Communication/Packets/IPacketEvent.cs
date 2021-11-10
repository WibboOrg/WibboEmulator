using Butterfly.Communication.Packets.Incoming;
using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient session, ClientPacket packet);
    }
}
