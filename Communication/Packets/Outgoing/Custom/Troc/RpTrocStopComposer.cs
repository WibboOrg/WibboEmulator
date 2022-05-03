namespace Butterfly.Communication.Packets.Outgoing.Custom.Troc
{
    internal class RpTrocStopComposer : ServerPacket
    {
        public RpTrocStopComposer()
          : base(ServerPacketHeader.RP_TROC_STOP)
        {
        }
    }
}
