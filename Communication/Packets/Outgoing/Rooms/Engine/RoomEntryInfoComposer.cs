namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class RoomEntryInfoComposer : ServerPacket
{
    public RoomEntryInfoComposer(int roomID, bool isOwner)
        : base(ServerPacketHeader.ROOM_INFO_OWNER)
    {
        this.WriteInteger(roomID);
        this.WriteBoolean(isOwner);
    }
}
