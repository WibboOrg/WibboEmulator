namespace WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Users;

internal sealed class UserBadgesComposer : ServerPacket
{
    public UserBadgesComposer(User user)
        : base(ServerPacketHeader.USER_BADGES_CURRENT)
    {
        this.WriteInteger(user.Id);
        this.WriteInteger(user.BadgeComponent.EquippedCount);

        foreach (var badge in user.BadgeComponent.GetBadges().ToList())
        {
            if (badge.Slot > 0)
            {
                this.WriteInteger(badge.Slot);
                this.WriteString(badge.Code);
            }
        }
    }
}
