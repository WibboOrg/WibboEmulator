using Wibbo.Communication.Packets.Outgoing.Messenger;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetBuddyRequestsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new BuddyRequestsComposer(Session.GetUser().GetMessenger().Requests));
        }
    }
}
