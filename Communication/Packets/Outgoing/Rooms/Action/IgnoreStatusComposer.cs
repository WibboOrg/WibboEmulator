namespace Butterfly.Communication.Packets.Outgoing.Rooms.Action
{
    internal class IgnoreStatusComposer : ServerPacket
    {
        public IgnoreStatusComposer()
            : base(ServerPacketHeader.USER_IGNORED_UPDATE)
        {

        }
    }
}
