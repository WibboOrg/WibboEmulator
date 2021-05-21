namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class IgnoreStatusMessageComposer : ServerPacket
    {
        public IgnoreStatusMessageComposer()
            : base(ServerPacketHeader.USER_IGNORED_UPDATE)
        {

        }
    }
}
