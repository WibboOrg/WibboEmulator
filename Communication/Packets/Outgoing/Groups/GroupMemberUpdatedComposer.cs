namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Users;

internal sealed class GroupMemberUpdatedComposer : ServerPacket
{
    public GroupMemberUpdatedComposer(int groupId, User user, int type)
        : base(ServerPacketHeader.GROUP_MEMBER)
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
