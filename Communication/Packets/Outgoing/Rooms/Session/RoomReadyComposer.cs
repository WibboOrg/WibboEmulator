namespace Butterfly.Communication.Packets.Outgoing.Rooms.Session
{
    internal class RoomReadyComposer : ServerPacket
    {
        public RoomReadyComposer(int RoomId, string Model)
            : base(ServerPacketHeader.ROOM_MODEL_NAME)
        {
            this.WriteString(Model);
            this.WriteInteger(RoomId);
        }
    }
}
