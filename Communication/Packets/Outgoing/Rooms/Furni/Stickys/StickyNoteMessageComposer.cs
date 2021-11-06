namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    internal class StickyNoteMessageComposer : ServerPacket
    {
        public StickyNoteMessageComposer(string ItemId, string Extradata)
            : base(ServerPacketHeader.FURNITURE_ITEMDATA)
        {
            WriteString(ItemId);
            WriteString(Extradata);
        }
    }
}
