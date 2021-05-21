namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FlatControllerAddedMessageComposer : ServerPacket
    {
        public FlatControllerAddedMessageComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD)
        {

        }
    }
}
