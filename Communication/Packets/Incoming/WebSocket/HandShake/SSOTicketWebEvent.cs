using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class SSOTicketWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {            if (Session == null)
            {
                return;
            }

            string SSOTicket = Packet.PopString();

            Session.TryAuthenticate(SSOTicket);
        }
    }
}
