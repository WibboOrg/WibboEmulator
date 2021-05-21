namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class StickyNoteMessageComposer : ServerPacket
    {
        public StickyNoteMessageComposer()
            : base(ServerPacketHeader.FURNITURE_ITEMDATA)
        {

        }
    }
}
