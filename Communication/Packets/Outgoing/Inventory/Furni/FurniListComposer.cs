namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.Items;

internal class FurniListComposer : ServerPacket
{
    public FurniListComposer(ICollection<Item> Items, int pages, int page)
        : base(ServerPacketHeader.USER_FURNITURE)
    {
        this.WriteInteger(pages);//Pages
        this.WriteInteger(page);//Page?

        this.WriteInteger(Items.Count);
        foreach (var Item in Items)
        {
            this.WriteItem(Item);
        }
    }

    private void WriteItem(Item Item)
    {
        this.WriteInteger(Item.Id);
        this.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
        this.WriteInteger(Item.Id);
        this.WriteInteger(Item.GetBaseItem().SpriteId);

        ItemBehaviourUtility.GenerateExtradata(Item, this);

        this.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
        this.WriteBoolean(Item.GetBaseItem().AllowTrade);
        this.WriteBoolean(Item.Limited == 0 && Item.GetBaseItem().AllowInventoryStack);
        this.WriteBoolean(ItemUtility.IsRare(Item));
        this.WriteInteger(-1);//Seconds to expiration.
        this.WriteBoolean(true);
        this.WriteInteger(-1);//Item RoomId

        if (!Item.IsWallItem)
        {
            this.WriteString(string.Empty);
            this.WriteInteger(0);
        }
    }
}
