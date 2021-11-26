using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetWardrobeEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new WardrobeComposer(Session.GetHabbo().GetWardrobeComponent().GetWardrobes()));
        }
    }
}