namespace Butterfly.Communication.Packets.Outgoing.Handshake
{
    internal class AvailabilityStatusComposer : ServerPacket
    {
        public AvailabilityStatusComposer()
            : base(ServerPacketHeader.AVAILABILITY_STATUS)
        {
            this.WriteBoolean(true);
            this.WriteBoolean(false);
            this.WriteBoolean(true);
        }
    }
}
