using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RequestFurniInventoryEvent : IPacketEvent
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

            if (!Session.GetHabbo().GetInventoryComponent().inventoryDefined)
            {
                Session.GetHabbo().GetInventoryComponent().LoadInventory();
                Session.GetHabbo().GetInventoryComponent().inventoryDefined = true;
            }

            IEnumerable<Item> Items = Session.GetHabbo().GetInventoryComponent().GetWallAndFloor;
            Session.SendPacket(new FurniListComposer(Items.ToList(), 1, 0));
        }
    }
}