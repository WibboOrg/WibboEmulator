namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemRemoveMessageComposer : ServerPacket
    {
        public ItemRemoveMessageComposer()
            : base(ServerPacketHeader.ITEM_WALL_REMOVE)
        {

        }
    }
}
