namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemRemoveMessageComposer : ServerPacket
    {
        public ItemRemoveMessageComposer(int itemId, int ownerId)
            : base(ServerPacketHeader.ITEM_WALL_REMOVE)
        {
            this.WriteString(itemId.ToString());
            this.WriteInteger(ownerId);
        }
    }
}
