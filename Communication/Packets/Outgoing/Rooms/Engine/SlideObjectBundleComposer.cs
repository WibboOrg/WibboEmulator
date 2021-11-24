namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class SlideObjectBundleComposer : ServerPacket
    {
        public SlideObjectBundleComposer()
            : base(ServerPacketHeader.ROOM_ROLLING)
        {

        }
    }
}
