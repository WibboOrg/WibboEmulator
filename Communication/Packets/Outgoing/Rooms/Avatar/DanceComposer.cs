namespace Wibbo.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class DanceComposer : ServerPacket
    {
        public DanceComposer(int virtualId, int dance)
            : base(ServerPacketHeader.UNIT_DANCE)
        {
            this.WriteInteger(virtualId);
            this.WriteInteger(dance);
        }
    }
}
