namespace Wibbo.Communication.Packets.Outgoing.Navigator
{
    internal class RoomInfoUpdatedComposer : ServerPacket
    {
        public RoomInfoUpdatedComposer(int RoomId)
            : base(ServerPacketHeader.ROOM_SETTINGS_UPDATED)
        {
            this.WriteInteger(RoomId);
        }
    }
}
