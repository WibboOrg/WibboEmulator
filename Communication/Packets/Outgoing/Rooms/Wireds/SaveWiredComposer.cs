namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds;

internal sealed class SaveWiredComposer : ServerPacket
{
    public SaveWiredComposer()
        : base(ServerPacketHeader.WIRED_SAVE)
    {

    }
}
