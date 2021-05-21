namespace Butterfly.Communication.Packets.Outgoing.Structure
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
