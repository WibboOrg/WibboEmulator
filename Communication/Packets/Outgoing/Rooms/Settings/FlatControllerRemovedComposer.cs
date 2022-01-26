namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerRemovedMessageComposer : ServerPacket
    {
        public FlatControllerRemovedMessageComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE)
        {
            this.WriteInteger(RoomId);
            this.WriteInteger(UserId);
        }
    }
}
