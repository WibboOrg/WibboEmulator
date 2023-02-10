namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class RoomPropertyComposer : ServerPacket
{
    public RoomPropertyComposer(string name, string val)
        : base(ServerPacketHeader.ROOM_PAINT)
    {
        this.WriteString(name);
        this.WriteString(val);
    }
}
