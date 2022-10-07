namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.Items;

internal class FurniListComposer : ServerPacket
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
        this.WriteString(item.GetBaseItem().Type.ToString().ToUpper());
        this.WriteInteger(item.Id);
        this.WriteInteger(item.GetBaseItem().SpriteId);

        ItemBehaviourUtility.GenerateExtradata(item, this);

        this.WriteBoolean(item.GetBaseItem().AllowEcotronRecycle);
        this.WriteBoolean(item.GetBaseItem().AllowTrade);
        this.WriteBoolean(item.Limited == 0 && item.GetBaseItem().AllowInventoryStack);
        this.WriteBoolean(ItemUtility.IsRare(item));
        this.WriteInteger(-1);//Seconds to expiration.
        this.WriteBoolean(true);
        this.WriteInteger(-1);//Item RoomId

        if (!item.IsWallItem)
        {
            this.WriteString(string.Empty);
            this.WriteInteger(0);
        }
    }
}
