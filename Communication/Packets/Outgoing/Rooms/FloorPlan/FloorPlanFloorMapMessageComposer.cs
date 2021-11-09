namespace Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    internal class FloorPlanFloorMapMessageComposer : ServerPacket
    {
        public FloorPlanFloorMapMessageComposer()
            : base(ServerPacketHeader.ROOM_MODEL_BLOCKED_TILES)
        {

        }
    }
}
