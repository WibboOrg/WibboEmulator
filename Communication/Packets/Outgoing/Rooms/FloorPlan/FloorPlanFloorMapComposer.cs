namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanFloorMapComposer : ServerPacket
    {
        public FloorPlanFloorMapComposer()
            : base(ServerPacketHeader.ROOM_MODEL_BLOCKED_TILES)
        {

        }
    }
}
