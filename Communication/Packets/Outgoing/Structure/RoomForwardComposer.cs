namespace Butterfly.Communication.Packets.Outgoing.Structure
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
