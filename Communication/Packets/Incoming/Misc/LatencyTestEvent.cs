using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class LatencyTestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.CLIENT_LATENCY);
            Response.WriteInteger(Packet.PopInt());
            Session.SendPacket(Response);
        }
    }
}