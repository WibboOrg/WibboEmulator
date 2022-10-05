namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;

internal class RpTrocStopComposer : ServerPacket
{
    public RpTrocStopComposer()
      : base(ServerPacketHeader.RP_TROC_STOP)
    {
    }
}
