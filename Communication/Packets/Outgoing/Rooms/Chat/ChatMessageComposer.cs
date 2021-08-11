namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ChatMessageComposer : ServerPacket
    {
        public ChatMessageComposer()
            : base(ServerPacketHeader.UNIT_CHAT)
        {

        }
    }
}
