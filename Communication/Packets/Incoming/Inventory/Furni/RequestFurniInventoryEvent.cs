using Wibbo.Communication.Packets.Outgoing.Inventory.Furni;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class RequestFurniInventoryEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
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

            IEnumerable<Item> Items = Session.GetUser().GetInventoryComponent().GetWallAndFloor;
            Session.SendPacket(new FurniListComposer(Items.ToList(), 1, 0));
        }
    }
}