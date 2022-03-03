using Butterfly.Game.Users;
using Butterfly.Game.Users.Badges;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class UserBadgesComposer : ServerPacket
    {
        public UserBadgesComposer(User User)
            : base(ServerPacketHeader.USER_BADGES_CURRENT)
        {
            this.WriteInteger(User.Id);
            this.WriteInteger(User.GetBadgeComponent().EquippedCount);

            int BadgeCount = 0;
            foreach (Badge badge in User.GetBadgeComponent().GetBadges().ToList())
            {
                if (badge.Slot > 0)
                {
                    BadgeCount++;
                    if (BadgeCount > 5)
                    {
                        break;
                    }

                    this.WriteInteger(badge.Slot);
                    this.WriteString(badge.Code);
                }
            }
        }
    }
}
