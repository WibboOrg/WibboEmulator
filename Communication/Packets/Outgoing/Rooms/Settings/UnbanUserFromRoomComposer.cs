namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;

internal class UnbanUserFromRoomComposer : ServerPacket
{
    public UnbanUserFromRoomComposer(int roomId, int userId)
        : base(ServerPacketHeader.ROOM_BAN_REMOVE)
    {
        this.WriteInteger(roomId);
        this.WriteInteger(userId);
    }
}
