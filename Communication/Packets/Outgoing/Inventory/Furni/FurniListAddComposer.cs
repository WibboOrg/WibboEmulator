namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Items;

internal sealed class FurniListAddComposer : ServerPacket
{
    public FurniListAddComposer(Item item)
        : base(ServerPacketHeader.USER_FURNITURE_ADD)
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
        this.WriteBoolean(true);
        this.WriteInteger(-1);//Item RoomId

        if (!item.IsWallItem)
        {
            this.WriteString(string.Empty);
            this.WriteInteger(item.Extra);
        }
    }
}
