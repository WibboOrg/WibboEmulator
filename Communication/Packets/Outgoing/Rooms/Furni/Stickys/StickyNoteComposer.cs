namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    internal class StickyNoteComposer : ServerPacket
    {
        public StickyNoteComposer(int Id, string ExtraData)
            : base(ServerPacketHeader.FURNITURE_ITEMDATA)
        {
            this.WriteString(Id.ToString());
            this.WriteString(ExtraData);
        }
    }
}
