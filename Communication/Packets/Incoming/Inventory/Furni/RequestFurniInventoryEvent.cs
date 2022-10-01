using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RequestFurniInventoryEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.GetUser() == null)
            {
                return;
            }

            if (session.GetUser().GetInventoryComponent() == null)
            {
                return;
            }

            session.GetUser().GetInventoryComponent().LoadInventory();

            IEnumerable<Item> Items = session.GetUser().GetInventoryComponent().GetWallAndFloor;
            session.SendPacket(new FurniListComposer(Items.ToList(), 1, 0));
        }
    }
}