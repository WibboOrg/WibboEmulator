namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class SlideObjectBundleMessageComposer : ServerPacket
    {
        public SlideObjectBundleMessageComposer()
            : base(ServerPacketHeader.ROOM_ROLLING)
        {

        }
    }
}
