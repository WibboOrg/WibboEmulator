using Butterfly.Game.Guilds;
using Butterfly.Game.Users;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembersComposer : ServerPacket
    {
        public GroupMembersComposer(Guild Group, ICollection<User> Members, int MembersCount, int Page, bool Admin, int ReqType, string SearchVal)
            : base(ServerPacketHeader.GROUP_MEMBERS)
        {
            this.WriteInteger(Group.Id);
            this.WriteString(Group.Name);
            this.WriteInteger(Group.RoomId);
            this.WriteString(Group.Badge);
            this.WriteInteger(MembersCount);

            this.WriteInteger(Members.Count);
            if (MembersCount > 0)
            {
                foreach (User Data in Members)
                {
                    this.WriteInteger(Group.CreatorId == Data.Id ? 0 : Group.IsAdmin(Data.Id) ? 1 : Group.IsMember(Data.Id) ? 2 : 3);
                    this.WriteInteger(Data.Id);
                    this.WriteString(Data.Username);
                    this.WriteString(Data.Look);
                    this.WriteString(string.Empty);
                }
            }
            this.WriteBoolean(Admin);
            this.WriteInteger(14);
            this.WriteInteger(Page);
            this.WriteInteger(ReqType);
            this.WriteString(SearchVal);
        }
    }
}
