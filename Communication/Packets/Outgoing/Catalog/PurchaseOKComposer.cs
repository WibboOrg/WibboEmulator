namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Items;

internal class PurchaseOKComposer : ServerPacket
{
    public PurchaseOKComposer(CatalogItem item, ItemData baseItem)
        : base(ServerPacketHeader.CATALOG_PURCHASE_OK)
    {
        this.WriteInteger(baseItem.Id);
        this.WriteString(baseItem.ItemName);
        this.WriteBoolean(false);
        this.WriteInteger(item.CostCredits);
        this.WriteInteger(item.CostDuckets);
        this.WriteInteger(0);
        this.WriteBoolean(true);
        this.WriteInteger(1);
        this.WriteString(baseItem.Type.ToString().ToLower());
        this.WriteInteger(baseItem.SpriteId);
        this.WriteString("");
        this.WriteInteger(1);
        this.WriteInteger(0);
        this.WriteString("");
        this.WriteInteger(1);
    }

    public PurchaseOKComposer()
        : base(ServerPacketHeader.CATALOG_PURCHASE_OK)
    {
        this.WriteInteger(0);
        this.WriteString("");
        this.WriteBoolean(false);
        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteInteger(0);
        this.WriteBoolean(true);
        this.WriteInteger(1);
        this.WriteString("s");
        this.WriteInteger(0);
        this.WriteString("");
        this.WriteInteger(1);
        this.WriteInteger(0);
        this.WriteString("");
        this.WriteInteger(1);
    }
}
