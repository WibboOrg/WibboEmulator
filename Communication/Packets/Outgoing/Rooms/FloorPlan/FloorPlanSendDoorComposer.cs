namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorComposer : ServerPacket
    {
        public FloorPlanSendDoorComposer(int DoorX, int DoorY, int DoorOrientation)
            : base(ServerPacketHeader.ROOM_MODEL_DOOR)
        {
            this.WriteInteger(DoorX);
            this.WriteInteger(DoorY);
            this.WriteInteger(DoorOrientation);
        }
    }
}
