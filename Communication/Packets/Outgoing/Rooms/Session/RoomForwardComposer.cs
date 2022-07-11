namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Session
{
    internal class RoomForwardComposer : ServerPacket
    {
        public RoomForwardComposer(int roomId)
            : base(ServerPacketHeader.ROOM_FORWARD)
        {
            this.WriteInteger(roomId);
        }
    }
}
