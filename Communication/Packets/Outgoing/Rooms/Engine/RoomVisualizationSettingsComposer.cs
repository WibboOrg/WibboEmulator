namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomVisualizationSettingsComposer : ServerPacket
    {
        public RoomVisualizationSettingsComposer(int walls, int floor, bool hideWalls)
            : base(ServerPacketHeader.ROOM_THICKNESS)
        {
            this.WriteBoolean(hideWalls);
            this.WriteInteger(walls);
            this.WriteInteger(floor);
        }
    }
}
