using Butterfly.Game.Guilds;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Catalog
{
    internal class GroupFurniConfigComposer : ServerPacket
    {
        public GroupFurniConfigComposer(ICollection<Guild> groups)
            : base(ServerPacketHeader.GROUP_LIST)
        {
            this.WriteInteger(groups.Count);
            foreach (Guild group in groups)
            {
                this.WriteInteger(group.Id);
                this.WriteString(group.Name);
                this.WriteString(group.Badge);
                this.WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                this.WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                this.WriteBoolean(false);
                this.WriteInteger(group.CreatorId);
                this.WriteBoolean(group.ForumEnabled);
            }
        }
    }
}
