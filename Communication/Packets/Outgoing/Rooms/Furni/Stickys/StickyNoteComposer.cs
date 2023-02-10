namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Stickys;

internal sealed class StickyNoteComposer : ServerPacket
{
    public StickyNoteComposer(int id, string extraData)
        : base(ServerPacketHeader.FURNITURE_ITEMDATA)
    {
        this.WriteString(id.ToString());
        this.WriteString(extraData);
    }
}
