namespace Wibbo.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class UnbanUserFromRoomComposer : ServerPacket
    {
        public UnbanUserFromRoomComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.ROOM_BAN_REMOVE)
        {
            this.WriteInteger(RoomId);
            this.WriteInteger(UserId);
        }
    }
}
