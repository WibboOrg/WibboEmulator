namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;
using WibboEmulator.Games.GameClients;

internal sealed class DeleteFurniTypeInventoryEvent : IPacketEvent
{
    public double Delay => 1000;

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

        var itemId = packet.PopInt();

        var item = session.User.InventoryComponent.GetItem(itemId);

        if (item == null)
        {
            return;
        }

        session.User.InventoryComponent.DeleteItemByType(item.BaseItem);
    }
}
