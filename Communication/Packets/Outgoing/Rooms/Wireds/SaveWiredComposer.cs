namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Wireds
{
    internal class SaveWiredComposer : ServerPacket
    {
        public SaveWiredComposer()
            : base(ServerPacketHeader.WIRED_SAVE)
        {

        }
    }
}
