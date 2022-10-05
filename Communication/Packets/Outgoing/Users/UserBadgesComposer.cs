namespace WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Users;

internal class UserBadgesComposer : ServerPacket
{
    public UserBadgesComposer(User user)
        : base(ServerPacketHeader.USER_BADGES_CURRENT)
    {
        this.WriteInteger(user.Id);
        this.WriteInteger(user.GetBadgeComponent().EquippedCount);

        var badgeCount = 0;
        foreach (var badge in user.GetBadgeComponent().GetBadges().ToList())
        {
            if (badge.Slot > 0)
            {
                badgeCount++;
                if (badgeCount > 5)
                {
                    break;
                }

                this.WriteInteger(badge.Slot);
                this.WriteString(badge.Code);
            }
        }
    }
}
