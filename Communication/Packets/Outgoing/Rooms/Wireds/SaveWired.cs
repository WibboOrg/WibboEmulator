namespace Butterfly.Communication.Packets.Outgoing.Rooms.Wireds
{
    internal class SaveWired : ServerPacket
    {
        public SaveWired()
            : base(ServerPacketHeader.WIRED_SAVE)
        {

        }
    }
}
