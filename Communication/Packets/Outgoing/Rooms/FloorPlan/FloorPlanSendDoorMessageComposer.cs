namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorMessageComposer : ServerPacket
    {
        public FloorPlanSendDoorMessageComposer(int DoorX, int DoorY, int DoorDirection)
            : base(ServerPacketHeader.ROOM_MODEL_DOOR)
        {
            WriteInteger(DoorX);
            WriteInteger(DoorY);
            WriteInteger(DoorDirection);
        }
    }
}
