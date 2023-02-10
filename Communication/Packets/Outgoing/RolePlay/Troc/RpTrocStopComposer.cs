namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;

internal sealed class RpTrocStopComposer : ServerPacket
{
    public RpTrocStopComposer()
      : base(ServerPacketHeader.RP_TROC_STOP)
    {
    }
}
