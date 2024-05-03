namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Users.Badges;

internal sealed class UserBadgesComposer : ServerPacket
{
    public UserBadgesComposer(User user)
        : base(ServerPacketHeader.USER_BADGES_CURRENT)
    {

        var badgeList = new List<Badge>();
        foreach (var badge in user.BadgeComponent.Badges.ToList())
        {
            if (badge.Slot == 0)
            {
                continue;
            }

            badgeList.Add(badge);
        }

        this.WriteInteger(user.Id);
        this.WriteInteger(badgeList.Count);

        foreach (var badge in badgeList.OrderBy(x => x.Slot))
        {
            this.WriteInteger(badge.Slot);
            this.WriteString(badge.Code);
        }
    }
}
