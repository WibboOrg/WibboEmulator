using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient session, ClientPacket packet);

        double Delay { get; }
    }
}
