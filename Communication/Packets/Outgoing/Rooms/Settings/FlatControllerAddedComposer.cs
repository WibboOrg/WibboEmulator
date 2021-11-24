namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerAddedComposer : ServerPacket
    {
        public FlatControllerAddedComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD)
        {

        }
    }
}
