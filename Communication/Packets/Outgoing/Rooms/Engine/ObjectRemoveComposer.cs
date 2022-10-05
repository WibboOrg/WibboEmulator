namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal class ObjectRemoveComposer : ServerPacket
{
    public ObjectRemoveComposer(int itemId, int ownerId)
        : base(ServerPacketHeader.FURNITURE_FLOOR_REMOVE)
    {
        this.WriteString(itemId.ToString());
        this.WriteBoolean(false); //isExpired
        this.WriteInteger(ownerId);
        this.WriteInteger(0);
    }
}
