namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FloorHeightMapMessageComposer : ServerPacket
    {
        public FloorHeightMapMessageComposer()
            : base(ServerPacketHeader.ROOM_MODEL)
        {

        }
    }
}
