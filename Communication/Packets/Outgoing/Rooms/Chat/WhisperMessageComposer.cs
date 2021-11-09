namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class WhisperMessageComposer : ServerPacket
    {
        public WhisperMessageComposer()
            : base(ServerPacketHeader.UNIT_CHAT_WHISPER)
        {

        }
    }
}
