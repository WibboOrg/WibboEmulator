namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class SlideObjectBundleMessageComposer : ServerPacket
    {
        public SlideObjectBundleMessageComposer()
            : base(ServerPacketHeader.ROOM_ROLLING)
        {

        }
    }
}
