using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetBuddyRequestsEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new BuddyRequestsComposer(Session.GetHabbo().GetMessenger().Requests));
        }
    }
}
