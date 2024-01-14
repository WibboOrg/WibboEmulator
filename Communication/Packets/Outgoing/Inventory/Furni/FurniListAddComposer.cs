namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Items;

internal sealed class FurniListAddComposer : ServerPacket
{
    public FurniListAddComposer(Item item)
        : base(ServerPacketHeader.USER_FURNITURE_ADD)
    {
        this.WriteInteger(item.Id);
        this.WriteString(item.GetBaseItem().Type.ToString());
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
