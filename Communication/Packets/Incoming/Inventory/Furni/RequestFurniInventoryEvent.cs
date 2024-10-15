namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;

internal sealed class RequestFurniInventoryEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (Session.User.InventoryComponent == null)
        {
            return;
        }

        Session.User.InventoryComponent.LoadInventory();

        var items = Session.User.InventoryComponent.GetWallAndFloor;
        Session.SendPacket(new FurniListComposer(items.ToList(), 1, 0));
    }
}
