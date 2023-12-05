namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;

internal sealed class ReceiveBadgeComposer : ServerPacket
{
    public ReceiveBadgeComposer(int badgeId, string badgeCode)
        : base(ServerPacketHeader.USER_BADGES_ADD)
    {
        this.WriteInteger(badgeId);
        this.WriteString(badgeCode);
    }
}
