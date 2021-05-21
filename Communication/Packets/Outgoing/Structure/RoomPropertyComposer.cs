namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomPropertyComposer : ServerPacket
    {
        public RoomPropertyComposer(string name, string val)
            : base(ServerPacketHeader.ROOM_PAINT)
        {
            this.WriteString(name);
            this.WriteString(val);
        }
    }
}
