namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal class ReceiveBadgeComposer : ServerPacket
{
    public ReceiveBadgeComposer(string badgeCode)
        : base(ServerPacketHeader.USER_BADGES_ADD)
    {
        this.WriteInteger(1);
        this.WriteString(badgeCode);
    }
}
