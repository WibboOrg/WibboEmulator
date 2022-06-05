using Wibbo.Communication.Packets.Outgoing.Handshake;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
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
