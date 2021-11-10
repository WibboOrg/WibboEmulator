using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class PingWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new PongComposer());
        }
    }
}
