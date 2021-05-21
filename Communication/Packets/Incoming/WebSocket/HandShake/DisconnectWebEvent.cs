using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class DisconnectWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            if (Session == null)
            {
                return;
            }

            Session.Disconnect();
        }
    }
}
