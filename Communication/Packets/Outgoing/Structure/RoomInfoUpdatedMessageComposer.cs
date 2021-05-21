namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomInfoUpdatedMessageComposer : ServerPacket
    {
        public RoomInfoUpdatedMessageComposer(int RoomId)
            : base(ServerPacketHeader.ROOM_SETTINGS_UPDATED)
        {
            this.WriteInteger(RoomId);
        }
    }
}
