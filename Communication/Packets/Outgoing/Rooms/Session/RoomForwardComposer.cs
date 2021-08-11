namespace Butterfly.Communication.Packets.Outgoing.Rooms.Session
{
    internal class RoomForwardComposer : ServerPacket
    {
        public RoomForwardComposer(int RoomId)
            : base(ServerPacketHeader.ROOM_FORWARD)
        {
            this.WriteInteger(RoomId);
        }
    }
}
