using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(Client session, ClientPacket packet);

        double Delay { get; }
    }
}
