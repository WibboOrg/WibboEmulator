namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer()
            : base(ServerPacketHeader.ROOM_HEIGHT_MAP)
        {

        }
    }
}
