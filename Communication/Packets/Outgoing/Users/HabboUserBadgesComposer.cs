using Butterfly.Game.Users.Badges;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class HabboUserBadgesComposer : ServerPacket
    {
        public HabboUserBadgesComposer(int Id, int EquippedCount, Dictionary<string, Badge> BadgeList)
            : base(ServerPacketHeader.USER_BADGES_CURRENT)
        {
            this.WriteInteger(Id);
            this.WriteInteger(EquippedCount);

            int BadgeCount = 0;
            foreach (Badge badge in BadgeList.Values)
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
