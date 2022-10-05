namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal class FloorHeightMapComposer : ServerPacket
{
    public FloorHeightMapComposer(int wallHeight, string mapFloor)
        : base(ServerPacketHeader.ROOM_MODEL)
    {
        this.WriteBoolean(wallHeight > 0);
        this.WriteInteger((wallHeight > 0) ? wallHeight : -1);
        this.WriteString(mapFloor);
    }
}
