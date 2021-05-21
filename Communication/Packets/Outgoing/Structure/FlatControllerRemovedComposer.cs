namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FlatControllerRemovedMessageComposer : ServerPacket
    {
        public FlatControllerRemovedMessageComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE)
        {

        }
    }
}
