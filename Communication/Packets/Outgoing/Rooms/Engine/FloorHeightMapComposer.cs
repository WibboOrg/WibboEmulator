namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FloorHeightMapComposer : ServerPacket
    {
        public FloorHeightMapComposer(int WallHeight, string MapFloor)
            : base(ServerPacketHeader.ROOM_MODEL)
        {
            this.WriteBoolean(WallHeight > 0);
            this.WriteInteger((WallHeight > 0) ? WallHeight : -1);
            this.WriteString(MapFloor);
        }
    }
}
