namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ShoutMessageComposer : ServerPacket
    {
        public ShoutMessageComposer()
            : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
        {

        }
    }
}
