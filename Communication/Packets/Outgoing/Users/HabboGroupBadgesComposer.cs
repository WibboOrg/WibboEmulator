using Butterfly.Game.Guilds;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class HabboGroupBadgesComposer : ServerPacket
    {
        public HabboGroupBadgesComposer(Dictionary<int, string> Badges)
            : base(ServerPacketHeader.GROUP_BADGES)
        {
            this.WriteInteger(Badges.Count);
            foreach (KeyValuePair<int, string> Badge in Badges)
            {
                this.WriteInteger(Badge.Key);
                this.WriteString(Badge.Value);
            }
        }

        public HabboGroupBadgesComposer(Guild Group)
            : base(ServerPacketHeader.GROUP_BADGES)
        {
            this.WriteInteger(1);//count
            {
                this.WriteInteger(Group.Id);
                this.WriteString(Group.Badge);
            }
        }
    }
}
