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
        this.WriteString(item.GetBaseItem().Type.ToString());
        this.WriteInteger(item.Id);
        this.WriteInteger(item.GetBaseItem().SpriteId);
        this.WriteInteger((int)item.Category);

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteBoolean(item.GetBaseItem().AllowEcotronRecycle);
        this.WriteBoolean(item.GetBaseItem().AllowTrade);
        this.WriteBoolean(item.Limited == 0 && item.GetBaseItem().AllowInventoryStack);
        this.WriteBoolean(ItemUtility.IsRare(item) && item.GetBaseItem().AllowMarketplaceSell);
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
