namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Stickys;

internal class StickyNoteComposer : ServerPacket
{
    public StickyNoteComposer(int id, string extraData)
        : base(ServerPacketHeader.FURNITURE_ITEMDATA)
    {
        this.WriteString(id.ToString());
        this.WriteString(extraData);
    }
}
