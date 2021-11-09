namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    internal class StickyNoteMessageComposer : ServerPacket
    {
        public StickyNoteMessageComposer()
            : base(ServerPacketHeader.FURNITURE_ITEMDATA)
        {

        }
    }
}
