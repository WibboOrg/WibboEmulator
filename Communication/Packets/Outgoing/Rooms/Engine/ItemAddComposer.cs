namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;

internal class ItemAddComposer : ServerPacket
{
    public ItemAddComposer(Item item, string userName, int userId)
        : base(ServerPacketHeader.ITEM_WALL_ADD)
    {
        this.WriteString(item.Id.ToString());
        this.WriteInteger(item.GetBaseItem().SpriteId);
        this.WriteString(item.WallCoord ?? string.Empty);

        ItemBehaviourUtility.GenerateWallExtradata(item, this);

        this.WriteInteger(-1);
        this.WriteInteger((item.GetBaseItem().Modes > 1) ? 1 : 0);
        this.WriteInteger(userId);
        this.WriteString(userName);
    }
}
