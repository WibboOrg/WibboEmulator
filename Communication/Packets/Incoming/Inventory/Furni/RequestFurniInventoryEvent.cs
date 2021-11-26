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

            Session.GetHabbo().GetInventoryComponent().LoadInventory();

            IEnumerable<Item> Items = Session.GetHabbo().GetInventoryComponent().GetWallAndFloor;
            Session.SendPacket(new FurniListComposer(Items.ToList(), 1, 0));
        }
    }
}