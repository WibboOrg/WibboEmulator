namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ShoutMessageComposer : ServerPacket
    {
        public ShoutMessageComposer()
            : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
        {

        }
    }
}
