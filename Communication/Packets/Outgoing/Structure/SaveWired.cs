namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class SaveWired : ServerPacket
    {
        public SaveWired()
            : base(ServerPacketHeader.WIRED_SAVE)
        {

        }
    }
}
