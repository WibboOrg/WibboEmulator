namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorComposer : ServerPacket
    {
        public FloorPlanSendDoorComposer()
            : base(ServerPacketHeader.ROOM_MODEL_DOOR)
        {

        }
    }
}
