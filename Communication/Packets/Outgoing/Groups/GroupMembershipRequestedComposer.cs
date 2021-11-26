using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembershipRequestedComposer : ServerPacket
    {
        public GroupMembershipRequestedComposer(int GroupId, User Habbo, int Type)
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
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
