namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectRemoveComposer : ServerPacket
    {
        public ObjectRemoveComposer(int ItemId, int OwnerId)
            : base(ServerPacketHeader.FURNITURE_FLOOR_REMOVE)
        {
            this.WriteString(ItemId.ToString());
            this.WriteBoolean(false); //isExpired
            this.WriteInteger(OwnerId);
            this.WriteInteger(0);
        }
    }
}
