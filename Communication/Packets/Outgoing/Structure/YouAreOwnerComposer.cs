namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class YouAreOwnerComposer : ServerPacket
    {
        public YouAreOwnerComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_OWNER)
        {

        }
    }
}
