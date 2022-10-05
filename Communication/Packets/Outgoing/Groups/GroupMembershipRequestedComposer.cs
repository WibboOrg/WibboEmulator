namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Users;

internal class GroupMembershipRequestedComposer : ServerPacket
{
    public GroupMembershipRequestedComposer(int groupId, User user, int type)
        : base(ServerPacketHeader.GROUP_MEMBERSHIP_REQUESTED_MESSAGE_COMPOSER)
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
