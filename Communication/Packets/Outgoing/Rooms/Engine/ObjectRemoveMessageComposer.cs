namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectRemoveMessageComposer : ServerPacket
    {
        public ObjectRemoveMessageComposer(int ItemId, int OwnerId)
            : base(ServerPacketHeader.FURNITURE_FLOOR_REMOVE)
        {
            this.WriteString(ItemId.ToString());
            this.WriteBoolean(false); //isExpired
            this.WriteInteger(OwnerId);
            this.WriteInteger(0);
        }
    }
}
