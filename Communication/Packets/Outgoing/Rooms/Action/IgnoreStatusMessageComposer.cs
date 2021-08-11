namespace Butterfly.Communication.Packets.Outgoing.Rooms.Action
{
    internal class IgnoreStatusMessageComposer : ServerPacket
    {
        public IgnoreStatusMessageComposer()
            : base(ServerPacketHeader.USER_IGNORED_UPDATE)
        {

        }
    }
}
