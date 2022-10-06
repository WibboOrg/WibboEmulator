namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.FloorPlan;

internal class FloorPlanSendDoorComposer : ServerPacket
{
    public FloorPlanSendDoorComposer(int doorX, int doorY, int doorOrientation)
        : base(ServerPacketHeader.ROOM_MODEL_DOOR)
    {
        this.WriteInteger(doorX);
        this.WriteInteger(doorY);
        this.WriteInteger(doorOrientation);
    }
}
