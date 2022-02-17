using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetPetInventoryEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().GetInventoryComponent() == null)
            {
                return;
            }

            Session.GetHabbo().GetInventoryComponent().LoadInventory();

            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
        }
    }
}