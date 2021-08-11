namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerAddedMessageComposer : ServerPacket
    {
        public FlatControllerAddedMessageComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD)
        {

        }
    }
}
