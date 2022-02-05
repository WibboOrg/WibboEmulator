using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new LatencyResponseComposer(Packet.PopInt()));
        }
    }
}