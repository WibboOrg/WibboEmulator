namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanSendDoorMessageComposer : ServerPacket
    {
        public FloorPlanSendDoorMessageComposer()
            : base(ServerPacketHeader.ROOM_MODEL_DOOR)
        {

        }
    }
}
