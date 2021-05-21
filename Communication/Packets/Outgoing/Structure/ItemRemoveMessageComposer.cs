namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ItemRemoveMessageComposer : ServerPacket
    {
        public ItemRemoveMessageComposer()
            : base(ServerPacketHeader.ITEM_WALL_REMOVE)
        {

        }
    }
}
