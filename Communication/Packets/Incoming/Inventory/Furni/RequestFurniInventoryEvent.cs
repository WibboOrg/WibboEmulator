namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;

internal class RequestFurniInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().InventoryComponent == null)
        {
            return;
        }

        session.GetUser().
        InventoryComponent.LoadInventory();

        var items = session.GetUser().InventoryComponent.GetWallAndFloor;
        session.SendPacket(new FurniListComposer(items.ToList(), 1, 0));
    }
}
