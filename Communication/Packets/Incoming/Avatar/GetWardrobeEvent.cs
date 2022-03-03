using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetWardrobeEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new WardrobeComposer(Session.GetUser().GetWardrobeComponent().GetWardrobes()));
        }
    }
}