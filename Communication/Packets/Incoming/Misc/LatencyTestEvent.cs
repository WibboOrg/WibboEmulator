using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.CLIENT_LATENCY);
            Response.WriteInteger(Packet.PopInt());
            Session.SendPacket(Response);
        }
    }
}