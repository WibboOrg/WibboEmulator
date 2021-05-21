namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class UserTypingMessageComposer : ServerPacket
    {
        public UserTypingMessageComposer()
            : base(ServerPacketHeader.UNIT_TYPING)
        {

        }
    }
}
