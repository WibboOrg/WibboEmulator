using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetPetInventoryEvent : IPacketEvent
    {
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

            if (!Session.GetHabbo().GetInventoryComponent().InventoryDefined)
            {
                Session.GetHabbo().GetInventoryComponent().LoadInventory();
                Session.GetHabbo().GetInventoryComponent().InventoryDefined = true;
            }

            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
        }
    }
}