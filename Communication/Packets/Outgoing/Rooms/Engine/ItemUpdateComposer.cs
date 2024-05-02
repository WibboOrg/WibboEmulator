namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;

internal sealed class ItemUpdateComposer : ServerPacket
{
    public ItemUpdateComposer(Item item, int userId)
        : base(ServerPacketHeader.ITEM_WALL_UPDATE) => this.WriteWallItem(item, userId);

    private void WriteWallItem(Item item, int userId)
    {
        this.WriteString(item.Id.ToString());
        this.WriteInteger(item.ItemData.SpriteId);
        this.WriteString(item.WallCoord);
        switch (item.ItemData.InteractionType)
        {
            case InteractionType.POSTIT:
                this.WriteString(item.ExtraData.Split(' ')[0]);
                break;

            default:
                this.WriteString(item.ExtraData);
                break;
        }
        this.WriteInteger(-1);
        this.WriteInteger((item.ItemData.Modes > 1) ? 1 : 0);
        this.WriteInteger(userId);
        this.WriteString("");
    }
}
