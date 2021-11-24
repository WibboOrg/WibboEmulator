namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FloorHeightMapComposer : ServerPacket
    {
        public FloorHeightMapComposer()
            : base(ServerPacketHeader.ROOM_MODEL)
        {

        }
    }
}
