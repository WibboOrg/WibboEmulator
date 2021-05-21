namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomVisualizationSettingsComposer : ServerPacket
    {
        public RoomVisualizationSettingsComposer(int Walls, int Floor, bool HideWalls)
            : base(ServerPacketHeader.ROOM_THICKNESS)
        {
            this.WriteBoolean(HideWalls);
            this.WriteInteger(Walls);
            this.WriteInteger(Floor);
        }
    }
}
