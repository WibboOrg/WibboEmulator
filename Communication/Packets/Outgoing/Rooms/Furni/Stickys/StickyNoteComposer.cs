namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    internal class StickyNoteComposer : ServerPacket
    {
        public StickyNoteComposer()
            : base(ServerPacketHeader.FURNITURE_ITEMDATA)
        {

        }
    }
}
