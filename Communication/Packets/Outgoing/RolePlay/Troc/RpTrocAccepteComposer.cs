namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;

internal sealed class RpTrocAccepteComposer : ServerPacket
{
    public RpTrocAccepteComposer(int userId, bool isAccepted)
      : base(ServerPacketHeader.RP_TROC_ACCEPTE)
    {
        this.WriteInteger(userId);
        this.WriteBoolean(isAccepted);
    }
}
