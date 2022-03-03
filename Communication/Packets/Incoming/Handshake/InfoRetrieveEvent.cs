using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InfoRetrieveEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new UserObjectComposer(Session.GetUser()));
            Session.SendPacket(new UserPerksComposer(Session.GetUser()));
        }
    }
}
