namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer(int x, int y, int nextX, int nextY)
            : base(ServerPacketHeader.ROOM_ROLLING)
        {

        }
    }
}
