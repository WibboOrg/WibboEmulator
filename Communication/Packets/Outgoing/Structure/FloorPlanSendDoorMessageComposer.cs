namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FloorPlanSendDoorMessageComposer : ServerPacket
    {
        public FloorPlanSendDoorMessageComposer()
            : base(ServerPacketHeader.ROOM_MODEL_DOOR)
        {

        }
    }
}
