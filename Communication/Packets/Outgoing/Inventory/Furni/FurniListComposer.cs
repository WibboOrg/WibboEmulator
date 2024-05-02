namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Items;

internal sealed class FurniListComposer : ServerPacket
{
    public FurniListComposer(ICollection<Item> items, int pages, int page)
        : base(ServerPacketHeader.USER_FURNITURE)
    {
        this.WriteInteger(pages);//Pages
        this.WriteInteger(page);//Page?

        this.WriteInteger(items.Count);
        foreach (var item in items)
        {
            this.WriteItem(item);
        }
    }

    private void WriteItem(Item item)
    {
        this.WriteInteger(item.Id);
        this.WriteString(item.ItemData.Type.ToString());
        this.WriteInteger(item.Id);
        this.WriteInteger(item.ItemData.SpriteId);
        this.WriteInteger((int)item.Category);

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteBoolean(item.ItemData.AllowEcotronRecycle);
        this.WriteBoolean(item.ItemData.AllowTrade);
        this.WriteBoolean(item.Limited == 0 && item.ItemData.AllowInventoryStack);
        this.WriteBoolean(ItemUtility.IsRare(item) && item.ItemData.AllowMarketplaceSell);
        this.WriteInteger(-1);//Seconds to expiration.
        this.WriteBoolean(false);
        this.WriteInteger(item.Id);//Item RoomId

        if (!item.IsWallItem)
        {
            this.WriteString(string.Empty);
            this.WriteInteger(item.Extra);
        }
    }
}
