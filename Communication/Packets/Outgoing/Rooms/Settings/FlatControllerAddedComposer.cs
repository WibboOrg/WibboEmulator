namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;

internal sealed class FlatControllerAddedComposer : ServerPacket
{
    public FlatControllerAddedComposer(int roomId, int userId, string userName)
        : base(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD)
    {
        this.WriteInteger(roomId);
        this.WriteInteger(userId);
        this.WriteString(userName);
    }
}
