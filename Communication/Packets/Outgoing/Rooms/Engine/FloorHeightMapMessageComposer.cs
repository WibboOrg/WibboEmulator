namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class FloorHeightMapMessageComposer : ServerPacket
    {
        public FloorHeightMapMessageComposer()
            : base(ServerPacketHeader.ROOM_MODEL)
        {

        }
    }
}
