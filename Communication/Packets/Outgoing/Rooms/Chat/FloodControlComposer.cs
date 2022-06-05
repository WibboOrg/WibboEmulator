namespace Wibbo.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class FloodControlComposer : ServerPacket
    {
        public FloodControlComposer(int FloodTime)
            : base(ServerPacketHeader.FLOOD_CONTROL)
        {
            this.WriteInteger(FloodTime);
        }
    }
}
