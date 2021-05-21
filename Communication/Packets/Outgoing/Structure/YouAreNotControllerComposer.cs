namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class YouAreNotControllerComposer : ServerPacket
    {
        public YouAreNotControllerComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_CLEAR)
        {

        }
    }
}
