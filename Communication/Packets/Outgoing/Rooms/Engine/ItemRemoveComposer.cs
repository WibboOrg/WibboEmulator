namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class ItemRemoveComposer : ServerPacket
{
    public ItemRemoveComposer(int itemId, int ownerId)
        : base(ServerPacketHeader.ITEM_WALL_REMOVE)
    {
        this.WriteString(itemId.ToString());
        this.WriteInteger(ownerId);
    }
}
