namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;

internal class RequestFurniInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (session.User.InventoryComponent == null)
        {
            return;
        }

        session.User.InventoryComponent.LoadInventory();

        var items = session.User.InventoryComponent.GetWallAndFloor;
        session.SendPacket(new FurniListComposer(items.ToList(), 1, 0));
    }
}
