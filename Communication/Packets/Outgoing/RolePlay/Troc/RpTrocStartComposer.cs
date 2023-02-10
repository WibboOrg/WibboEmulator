namespace WibboEmulator.Communication.Packets.Outgoing.RolePlay.Troc;

internal sealed class RpTrocStartComposer : ServerPacket
{
    public RpTrocStartComposer(int userId, string username)
      : base(ServerPacketHeader.RP_TROC_START)
    {
        this.WriteInteger(userId);
        this.WriteString(username);
    }
}
