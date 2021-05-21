namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomEntryInfoComposer : ServerPacket
    {
        public RoomEntryInfoComposer(int roomID, bool isOwner)
            : base(ServerPacketHeader.ROOM_INFO_OWNER)
        {
            this.WriteInteger(roomID);
            this.WriteBoolean(isOwner);
        }
    }
}
