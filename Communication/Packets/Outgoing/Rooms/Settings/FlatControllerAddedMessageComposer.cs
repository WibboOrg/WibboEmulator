namespace Butterfly.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerAddedMessageComposer : ServerPacket
    {
        public FlatControllerAddedMessageComposer(int RoomId, int UserId, string Username)
            : base(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD)
        {
            WriteInteger(RoomId);
            WriteInteger(UserId);
            WriteString(Username);

        }
    }
}
