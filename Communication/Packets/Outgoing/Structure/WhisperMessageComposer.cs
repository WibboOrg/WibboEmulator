namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class WhisperMessageComposer : ServerPacket
    {
        public WhisperMessageComposer()
            : base(ServerPacketHeader.UNIT_CHAT_WHISPER)
        {

        }
    }
}
