namespace Wibbo.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerAddedComposer : ServerPacket
    {
        public FlatControllerAddedComposer(int RoomId, int UserId, string UserName)
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD)
        {
            this.WriteInteger(RoomId);
            this.WriteInteger(UserId);
            this.WriteString(UserName);
        }
    }
}
