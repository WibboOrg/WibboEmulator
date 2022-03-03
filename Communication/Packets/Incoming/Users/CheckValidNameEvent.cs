using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CheckValidNameEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null || Session == null)
            {
                return;
            }

            string Name = Packet.PopString();

            Session.SendPacket(new NameChangeUpdateComposer(Name));
        }
    }
}