namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;

using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;

internal sealed class DeleteFurniTypeInventoryEvent : IPacketEvent
{
    public double Delay => 1000;

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

        var itemId = packet.PopInt();

        var item = Session.User.InventoryComponent.GetItem(itemId);

        if (item == null)
        {
            return;
        }

        if (item.ItemData.IsRare && !Session.User.HasPermission("empty_items_all"))
        {
            return;
        }

        var items = Session.User.InventoryComponent.GetItemsByType(item.BaseItemId);

        using var dbClient = DatabaseManager.Connection;
        Session.User.InventoryComponent.DeleteItems(dbClient, items, item.BaseItemId);
    }
}
