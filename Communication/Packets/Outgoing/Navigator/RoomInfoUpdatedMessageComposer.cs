namespace Butterfly.Communication.Packets.Outgoing.Navigator
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
