namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer()
            : base(ServerPacketHeader.ROOM_HEIGHT_MAP)
        {

        }
    }
}
