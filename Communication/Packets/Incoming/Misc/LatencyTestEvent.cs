using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new LatencyResponseComposer(Packet.PopInt()));
        }
    }
}