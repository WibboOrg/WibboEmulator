namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class CanCreateRoomComposer : ServerPacket
    {
        public CanCreateRoomComposer(bool Error, int MaxRoomsPerUser)
            : base(ServerPacketHeader.CanCreateRoomMessageComposer)
        {
            this.WriteInteger(Error ? 1 : 0);
            this.WriteInteger(MaxRoomsPerUser);
        }
    }
}
