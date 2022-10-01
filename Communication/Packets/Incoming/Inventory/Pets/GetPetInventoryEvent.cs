using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetPetInventoryEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            if (Session.GetUser().GetInventoryComponent() == null)
            {
                return;
            }

            Session.GetUser().GetInventoryComponent().LoadInventory();

            Session.SendPacket(new PetInventoryComposer(Session.GetUser().GetInventoryComponent().GetPets()));
        }
    }
}