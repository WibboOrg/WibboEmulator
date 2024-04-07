namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Items;

internal sealed class TradingUpdateComposer : ServerPacket
{
    public TradingUpdateComposer(int userOneId, List<Item> userOneItems, int userTwoId, List<Item> userTwoItems)
        : base(ServerPacketHeader.TRADE_LIST_ITEM)
    {
        this.WriteInteger(userOneId);
        this.WriteInteger(userOneItems.Count);
        foreach (var userItem in userOneItems)
        {
            this.WriteItem(userItem);
        }
        this.WriteInteger(userOneItems.Count);
        this.WriteInteger(0);

        this.WriteInteger(userTwoId);
        this.WriteInteger(userTwoItems.Count);
        foreach (var userItem in userTwoItems)
        {
            this.WriteItem(userItem);
        }
        this.WriteInteger(userTwoItems.Count);
        this.WriteInteger(0);
    }

    private void WriteItem(Item userItem)
    {
        this.WriteInteger(userItem.Id);
        this.WriteString(userItem.GetBaseItem().Type.ToString());
        this.WriteInteger(userItem.Id);
        this.WriteInteger(userItem.GetBaseItem().SpriteId);
        this.WriteInteger((int)userItem.Category);

        this.WriteBoolean(userItem.GetBaseItem().AllowInventoryStack);
        ItemBehaviourUtility.GenerateExtradata(userItem, this);

        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteInteger(0);
        if (userItem.GetBaseItem().Type == ItemType.S)
        {
            this.WriteInteger(userItem.Extra);
        }
    }
}
