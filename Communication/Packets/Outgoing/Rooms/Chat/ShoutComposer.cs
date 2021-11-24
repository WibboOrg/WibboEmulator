namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ShoutComposer : ServerPacket
    {
        public ShoutComposer()
            : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
        {

        }
    }
}
