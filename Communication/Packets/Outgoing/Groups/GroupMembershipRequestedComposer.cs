using WibboEmulator.Game.Users;

namespace WibboEmulator.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembershipRequestedComposer : ServerPacket
    {
        public GroupMembershipRequestedComposer(int groupId, User user, int type)
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
        {
            this.WriteInteger(groupId);//GroupId
            this.WriteInteger(type);//Type?
            {
                this.WriteInteger(user.Id);//UserId
                this.WriteString(user.Username);
                this.WriteString(user.Look);
                this.WriteString(string.Empty);
            }
        }
    }
}
