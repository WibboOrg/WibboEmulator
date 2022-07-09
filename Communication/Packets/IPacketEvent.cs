using Wibbo.Communication.Packets.Incoming;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(Client session, ClientPacket packet);

        double Delay { get; }
    }
}
