using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetPetInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().GetInventoryComponent() == null)
            {
                return;
            }

            if (!Session.GetHabbo().GetInventoryComponent().inventoryDefined)
            {
                Session.GetHabbo().GetInventoryComponent().LoadInventory();
                Session.GetHabbo().GetInventoryComponent().inventoryDefined = true;
            }

            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
        }
    }
}