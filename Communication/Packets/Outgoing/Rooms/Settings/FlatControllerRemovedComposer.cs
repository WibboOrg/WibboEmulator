namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;

internal class FlatControllerRemovedMessageComposer : ServerPacket
{
    public FlatControllerRemovedMessageComposer(int roomId, int userId)
        : base(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE)
    {
        this.WriteInteger(roomId);
        this.WriteInteger(userId);
    }
}
