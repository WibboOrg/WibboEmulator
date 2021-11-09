namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerRemovedMessageComposer : ServerPacket
    {
        public FlatControllerRemovedMessageComposer()
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE)
        {

        }
    }
}
