using Butterfly.Communication.Packets.Outgoing.Catalog;

using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CheckPetNameEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            string PetName = Packet.PopString();

            Session.SendPacket(new CheckPetNameMessageComposer(PetName));
        }
    }
}