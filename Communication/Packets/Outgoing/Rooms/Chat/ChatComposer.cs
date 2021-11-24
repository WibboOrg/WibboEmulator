namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ChatComposer : ServerPacket
    {
        public ChatComposer()
            : base(ServerPacketHeader.UNIT_CHAT)
        {

        }
    }
}
