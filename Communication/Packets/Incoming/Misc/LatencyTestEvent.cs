using Wibbo.Communication.Packets.Outgoing.Misc;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new LatencyResponseComposer(Packet.PopInt()));
        }
    }
}