namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.session;

internal class RoomReadyComposer : ServerPacket
{
    public RoomReadyComposer(int roomId, string model)
        : base(ServerPacketHeader.ROOM_MODEL_NAME)
    {
        this.WriteString(model);
        this.WriteInteger(roomId);
    }
}
