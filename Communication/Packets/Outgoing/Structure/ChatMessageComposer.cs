namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ChatMessageComposer : ServerPacket
    {
        public ChatMessageComposer()
            : base(ServerPacketHeader.UNIT_CHAT)
        {

        }
    }
}
