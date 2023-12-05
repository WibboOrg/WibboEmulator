namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;

internal sealed class RemovedBadgeComposer : ServerPacket
{
    public RemovedBadgeComposer(string badgeCode)
        : base(ServerPacketHeader.USER_BADGE_REMOVE) => this.WriteString(badgeCode);
}
