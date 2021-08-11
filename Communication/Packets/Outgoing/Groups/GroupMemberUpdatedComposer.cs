using Butterfly.HabboHotel.Users;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class GroupMemberUpdatedComposer : ServerPacket
    {
        public GroupMemberUpdatedComposer(int GroupId, Habbo Habbo, int Type)
            : base(ServerPacketHeader.GROUP_MEMBER)
        {
            this.WriteInteger(GroupId);//GroupId
            this.WriteInteger(Type);//Type?
            {
                this.WriteInteger(Habbo.Id);//UserId
                this.WriteString(Habbo.Username);
                this.WriteString(Habbo.Look);
                this.WriteString(string.Empty);
            }
        }
    }
}
